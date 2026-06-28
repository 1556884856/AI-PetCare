using Microsoft.EntityFrameworkCore;
using PetCare.Core.Dtos;
using PetCare.Core.Entities;
using PetCare.Core.Events;
using PetCare.Core.Interfaces;
using PetCare.Data;
using PetCare.Core.Enums;
using PetCare.Service.Redis;
namespace PetCare.Service;
/// <summary>
/// 支付服务实现，管理支付单的创建、模拟支付和退款。
/// 创建支付时计算优惠券抵扣，模拟支付使用 Redis 分布式锁防止重复支付。
/// 支付完成后发布事件通知并更新优惠券使用状态。
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly PetCareDbContext _db;
    private readonly RedisConnection _redis;
    private readonly IMessageBus _messageBus;
    public PaymentService(PetCareDbContext db, RedisConnection redis, IMessageBus messageBus) { _db = db; _redis = redis; _messageBus = messageBus; }
    /// <summary>创建支付单，计算优惠券抵扣金额，发布30分钟超时事件</summary>
    public async Task<PaymentDto> CreatePaymentAsync(int userId, CreatePaymentRequest r)
    {
        var appointment = await _db.Appointments.Include(a => a.Service).Include(a => a.Pet).FirstOrDefaultAsync(a => a.Id == r.AppointmentId && a.UserId == userId) ?? throw new Exception("预约不存在");
        var existing = await _db.Payments.AnyAsync(p => p.AppointmentId == r.AppointmentId);
        if (existing) throw new Exception("该预约已有支付单");
        var amount = appointment.Service.Price;
        var discountAmount = 0m;
        var finalAmount = amount;
        int? couponId = null;
        // 应用优惠券
        if (r.CouponId.HasValue)
        {
            var userCoupon = await _db.UserCoupons.Include(uc => uc.Coupon).FirstOrDefaultAsync(uc => uc.Id == r.CouponId && uc.UserId == userId && !uc.IsUsed) ?? throw new Exception("优惠券不可用");
            var coupon = userCoupon.Coupon;
            if (coupon.ValidTo < DateTime.UtcNow) throw new Exception("优惠券已过期");
            if (coupon.MinOrderAmount > amount) throw new Exception($"未达到最低消费金额 ¥{coupon.MinOrderAmount}");
            couponId = coupon.Id;
            if ((int)coupon.Type == (int)CouponType.FixedAmount) discountAmount = coupon.Value;    // 满减券
            else discountAmount = amount - amount * coupon.Value;                                  // 折扣券
            finalAmount = amount - discountAmount;
            if (finalAmount < 0) finalAmount = 0;
        }
        var payment = new Payment { AppointmentId = r.AppointmentId, UserId = userId, Amount = amount, DiscountAmount = discountAmount, FinalAmount = finalAmount, CouponId = couponId, PayMethod = (PaymentMethod)r.PayMethod };
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();
        // 发布支付超时事件：30分钟后未支付则自动取消
        await _messageBus.PublishAsync("payment.timeout", new PaymentTimeoutEvent(payment.Id, r.AppointmentId, couponId));
        return MapPayment(payment, appointment.Service.Name, appointment.Pet.Name);
    }
    /// <summary>模拟支付（开发环境用），Redis 分布式锁防重复</summary>
    public async Task<PaymentDto> MockPayAsync(int userId, int paymentId)
    {
        var lockKey = $"payment:lock:{paymentId}";
        var acquired = await _redis.Database.StringSetAsync(lockKey, "1", TimeSpan.FromSeconds(30), StackExchange.Redis.When.NotExists);
        if (!acquired) throw new Exception("支付处理中，请勿重复操作");
        try
        {
            var payment = await _db.Payments.Include(p => p.Appointment).ThenInclude(a => a.Service).Include(p => p.Appointment).ThenInclude(a => a.Pet).FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId) ?? throw new Exception("支付单不存在");
            if (payment.Status != PaymentStatus.Pending) throw new Exception("支付单状态异常");
            await Task.Delay(1500);  // 模拟支付处理过程
            payment.Status = PaymentStatus.Paid;
            payment.PaidAt = DateTime.UtcNow;
            // 标记优惠券已使用
            if (payment.CouponId.HasValue)
            {
                var userCoupon = await _db.UserCoupons.FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == payment.CouponId.Value && !uc.IsUsed);
                if (userCoupon != null) { userCoupon.IsUsed = true; userCoupon.UsedAt = DateTime.UtcNow; }
            }
            payment.Appointment.Status = AppointmentStatus.Confirmed;
            await _db.SaveChangesAsync();
            await _messageBus.PublishAsync("payment.completed", new PaymentCompletedEvent(payment.Id, payment.AppointmentId, userId, payment.FinalAmount));
            return MapPayment(payment, payment.Appointment.Service.Name, payment.Appointment.Pet.Name);
        }
        finally { await _redis.Database.KeyDeleteAsync(lockKey); }
    }
    /// <summary>退款：恢复优惠券、取消预约</summary>
    public async Task<PaymentDto> RefundAsync(int paymentId)
    {
        var payment = await _db.Payments.Include(p => p.Appointment).ThenInclude(a => a.Service).Include(p => p.Appointment).ThenInclude(a => a.Pet).FirstOrDefaultAsync(p => p.Id == paymentId) ?? throw new Exception("支付单不存在");
        if (payment.Status != PaymentStatus.Paid) throw new Exception("仅已支付的订单可退款");
        payment.Status = PaymentStatus.Refunded;
        payment.Appointment.Status = AppointmentStatus.Cancelled;
        // 退还优惠券
        if (payment.CouponId.HasValue)
        {
            var userCoupon = await _db.UserCoupons.FirstOrDefaultAsync(uc => uc.UserId == payment.UserId && uc.CouponId == payment.CouponId.Value && uc.IsUsed);
            if (userCoupon != null) { userCoupon.IsUsed = false; userCoupon.UsedAt = null; }
        }
        await _db.SaveChangesAsync();
        return MapPayment(payment, payment.Appointment.Service.Name, payment.Appointment.Pet.Name);
    }
    public async Task<PaymentDto?> GetPaymentAsync(int paymentId)
    {
        var payment = await _db.Payments.Include(p => p.Appointment).ThenInclude(a => a.Service).Include(p => p.Appointment).ThenInclude(a => a.Pet).FirstOrDefaultAsync(p => p.Id == paymentId);
        return payment == null ? null : MapPayment(payment, payment.Appointment.Service.Name, payment.Appointment.Pet.Name);
    }
    public async Task<List<PaymentDto>> GetUserPaymentsAsync(int userId)
    {
        return await _db.Payments.Where(p => p.UserId == userId).Include(p => p.Appointment).ThenInclude(a => a.Service).Include(p => p.Appointment).ThenInclude(a => a.Pet).OrderByDescending(p => p.CreatedAt).Select(p => new PaymentDto(p.Id, p.AppointmentId, p.UserId, p.Amount, p.DiscountAmount, p.FinalAmount, p.CouponId, (int)p.Status, (int)p.PayMethod, p.PaidAt, p.CreatedAt, p.Appointment.Service.Name, p.Appointment.Pet.Name)).ToListAsync();
    }
    public async Task<List<PaymentDto>> GetAllPaymentsAsync()
    {
        return await _db.Payments.Include(p => p.Appointment).ThenInclude(a => a.Service).Include(p => p.Appointment).ThenInclude(a => a.Pet).OrderByDescending(p => p.CreatedAt).Select(p => new PaymentDto(p.Id, p.AppointmentId, p.UserId, p.Amount, p.DiscountAmount, p.FinalAmount, p.CouponId, (int)p.Status, (int)p.PayMethod, p.PaidAt, p.CreatedAt, p.Appointment.Service.Name, p.Appointment.Pet.Name)).ToListAsync();
    }
    private static PaymentDto MapPayment(Payment p, string serviceName, string petName) => new PaymentDto(p.Id, p.AppointmentId, p.UserId, p.Amount, p.DiscountAmount, p.FinalAmount, p.CouponId, (int)p.Status, (int)p.PayMethod, p.PaidAt, p.CreatedAt, serviceName, petName);
}
