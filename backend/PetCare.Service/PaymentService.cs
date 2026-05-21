
using Microsoft.EntityFrameworkCore;
using PetCare.Core.Dtos;
using PetCare.Core.Entities;
using PetCare.Core.Events;
using PetCare.Core.Interfaces;
using PetCare.Data;
using PetCare.Service.Redis;

namespace PetCare.Service;

public class PaymentService : IPaymentService
{
    private readonly PetCareDbContext _db;
    private readonly RedisConnection _redis;
    private readonly IMessageBus _messageBus;

    public PaymentService(PetCareDbContext db, RedisConnection redis, IMessageBus messageBus)
    {
        _db = db;
        _redis = redis;
        _messageBus = messageBus;
    }

    public async Task<PaymentDto> CreatePaymentAsync(int userId, CreatePaymentRequest r)
    {
        var appointment = await _db.Appointments
            .Include(a => a.Service).Include(a => a.Pet)
            .FirstOrDefaultAsync(a => a.Id == r.AppointmentId && a.UserId == userId)
            ?? throw new Exception("预约不存在");

        var existing = await _db.Payments.AnyAsync(p => p.AppointmentId == r.AppointmentId);
        if (existing) throw new Exception("该预约已有支付单");

        var amount = appointment.Service.Price;
        var discountAmount = 0m;
        var finalAmount = amount;
        int? couponId = null;

        // 应用优惠券
        if (r.CouponId.HasValue)
        {
            var userCoupon = await _db.UserCoupons
                .Include(uc => uc.Coupon)
                .FirstOrDefaultAsync(uc => uc.Id == r.CouponId && uc.UserId == userId && !uc.IsUsed)
                ?? throw new Exception("优惠券不可用");

            var coupon = userCoupon.Coupon;
            if (coupon.ValidTo < DateTime.UtcNow) throw new Exception("优惠券已过期");
            if (coupon.MinOrderAmount > amount) throw new Exception($"未满最低消费 ￥{coupon.MinOrderAmount}");

            couponId = coupon.Id;
            if (coupon.Type == 0) // 满减
            {
                discountAmount = coupon.Value;
            }
            else // 折扣
            {
                discountAmount = amount - amount * coupon.Value;
            }
            finalAmount = amount - discountAmount;
            if (finalAmount < 0) finalAmount = 0;
        }

        var payment = new Payment
        {
            AppointmentId = r.AppointmentId,
            UserId = userId,
            Amount = amount,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount,
            CouponId = couponId,
            PayMethod = r.PayMethod
        };
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();

        // 发送超时延迟消息（30分钟未支付自动取消）
        _messageBus.Publish("payment.timeout", new PaymentTimeoutEvent(payment.Id, r.AppointmentId, couponId));

        return MapPayment(payment, appointment.Service.Name, appointment.Pet.Name);
    }

    public async Task<PaymentDto> MockPayAsync(int userId, int paymentId)
    {
        // Redis 防重锁
        var lockKey = $"payment:lock:{paymentId}";
        var acquired = await _redis.Database.StringSetAsync(lockKey, "1", TimeSpan.FromSeconds(30), StackExchange.Redis.When.NotExists);
        if (!acquired) throw new Exception("支付处理中，请勿重复操作");

        try
        {
            var payment = await _db.Payments
                .Include(p => p.Appointment).ThenInclude(a => a.Service)
                .Include(p => p.Appointment).ThenInclude(a => a.Pet)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId)
                ?? throw new Exception("支付单不存在");

            if (payment.Status != 0) throw new Exception("支付单状态异常");

            // 模拟支付延迟
            await Task.Delay(1500);

            payment.Status = 1; // 已支付
            payment.PaidAt = DateTime.UtcNow;

            // 标记优惠券已使用
            if (payment.CouponId.HasValue)
            {
                var userCoupon = await _db.UserCoupons
                    .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == payment.CouponId.Value && !uc.IsUsed);
                if (userCoupon != null)
                {
                    userCoupon.IsUsed = true;
                    userCoupon.UsedAt = DateTime.UtcNow;
                }
            }

            // 更新预约状态为已确认
            payment.Appointment.Status = 1;

            await _db.SaveChangesAsync();

            // 发送支付成功事件，触发通知
            _messageBus.Publish("payment.completed", new PaymentCompletedEvent(payment.Id, payment.AppointmentId, userId, payment.FinalAmount));

            return MapPayment(payment, payment.Appointment.Service.Name, payment.Appointment.Pet.Name);
        }
        finally
        {
            await _redis.Database.KeyDeleteAsync(lockKey);
        }
    }

    public async Task<PaymentDto> RefundAsync(int paymentId)
    {
        var payment = await _db.Payments
            .Include(p => p.Appointment).ThenInclude(a => a.Service)
            .Include(p => p.Appointment).ThenInclude(a => a.Pet)
            .FirstOrDefaultAsync(p => p.Id == paymentId)
            ?? throw new Exception("支付单不存在");

        if (payment.Status != 1) throw new Exception("仅已支付订单可退款");

        payment.Status = 2; // 已退款
        payment.Appointment.Status = 3; // 预约取消

        // 退还优惠券
        if (payment.CouponId.HasValue)
        {
            var userCoupon = await _db.UserCoupons
                .FirstOrDefaultAsync(uc => uc.UserId == payment.UserId && uc.CouponId == payment.CouponId.Value && uc.IsUsed);
            if (userCoupon != null)
            {
                userCoupon.IsUsed = false;
                userCoupon.UsedAt = null;
            }
        }

        await _db.SaveChangesAsync();
        return MapPayment(payment, payment.Appointment.Service.Name, payment.Appointment.Pet.Name);
    }

    public async Task<PaymentDto?> GetPaymentAsync(int paymentId)
    {
        var payment = await _db.Payments
            .Include(p => p.Appointment).ThenInclude(a => a.Service)
            .Include(p => p.Appointment).ThenInclude(a => a.Pet)
            .FirstOrDefaultAsync(p => p.Id == paymentId);
        return payment == null ? null : MapPayment(payment, payment.Appointment.Service.Name, payment.Appointment.Pet.Name);
    }

    public async Task<List<PaymentDto>> GetUserPaymentsAsync(int userId)
    {
        return await _db.Payments
            .Where(p => p.UserId == userId)
            .Include(p => p.Appointment).ThenInclude(a => a.Service)
            .Include(p => p.Appointment).ThenInclude(a => a.Pet)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentDto(p.Id, p.AppointmentId, p.UserId, p.Amount, p.DiscountAmount, p.FinalAmount, p.CouponId, p.Status, p.PayMethod, p.PaidAt, p.CreatedAt, p.Appointment.Service.Name, p.Appointment.Pet.Name))
            .ToListAsync();
    }

    public async Task<List<PaymentDto>> GetAllPaymentsAsync()
    {
        return await _db.Payments
            .Include(p => p.Appointment).ThenInclude(a => a.Service)
            .Include(p => p.Appointment).ThenInclude(a => a.Pet)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentDto(p.Id, p.AppointmentId, p.UserId, p.Amount, p.DiscountAmount, p.FinalAmount, p.CouponId, p.Status, p.PayMethod, p.PaidAt, p.CreatedAt, p.Appointment.Service.Name, p.Appointment.Pet.Name))
            .ToListAsync();
    }

    private static PaymentDto MapPayment(Payment p, string serviceName, string petName)
    {
        return new PaymentDto(p.Id, p.AppointmentId, p.UserId, p.Amount, p.DiscountAmount, p.FinalAmount, p.CouponId, p.Status, p.PayMethod, p.PaidAt, p.CreatedAt, serviceName, petName);
    }
}
