
using Microsoft.EntityFrameworkCore;
using PetCare.Core.Dtos;
using PetCare.Core.Entities;
using PetCare.Core.Events;
using PetCare.Core.Interfaces;
using PetCare.Data;
using PetCare.Core.Enums;
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
            ?? throw new Exception("ԤԼ������");

        var existing = await _db.Payments.AnyAsync(p => p.AppointmentId == r.AppointmentId);
        if (existing) throw new Exception("��ԤԼ����֧����");

        var amount = appointment.Service.Price;
        var discountAmount = 0m;
        var finalAmount = amount;
        int? couponId = null;

        // Ӧ���Ż�ȯ
        if (r.CouponId.HasValue)
        {
            var userCoupon = await _db.UserCoupons
                .Include(uc => uc.Coupon)
                .FirstOrDefaultAsync(uc => uc.Id == r.CouponId && uc.UserId == userId && !uc.IsUsed)
                ?? throw new Exception("�Ż�ȯ������");

            var coupon = userCoupon.Coupon;
            if (coupon.ValidTo < DateTime.UtcNow) throw new Exception("�Ż�ȯ�ѹ���");
            if (coupon.MinOrderAmount > amount) throw new Exception($"δ��������� ��{coupon.MinOrderAmount}");

            couponId = coupon.Id;
            if ((int)coupon.Type == (int)CouponType.FixedAmount) // ����
            {
                discountAmount = coupon.Value;
            }
            else // �ۿ�
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
            PayMethod = (PaymentMethod)r.PayMethod
        };
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();

        // ���ͳ�ʱ�ӳ���Ϣ��30����δ֧���Զ�ȡ����
        await _messageBus.PublishAsync("payment.timeout", new PaymentTimeoutEvent(payment.Id, r.AppointmentId, couponId));

        return MapPayment(payment, appointment.Service.Name, appointment.Pet.Name);
    }

    public async Task<PaymentDto> MockPayAsync(int userId, int paymentId)
    {
        // Redis ������
        var lockKey = $"payment:lock:{paymentId}";
        var acquired = await _redis.Database.StringSetAsync(lockKey, "1", TimeSpan.FromSeconds(30), StackExchange.Redis.When.NotExists);
        if (!acquired) throw new Exception("֧�������У������ظ�����");

        try
        {
            var payment = await _db.Payments
                .Include(p => p.Appointment).ThenInclude(a => a.Service)
                .Include(p => p.Appointment).ThenInclude(a => a.Pet)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId)
                ?? throw new Exception("֧����������");

            if (payment.Status != PaymentStatus.Pending) throw new Exception("֧����״̬�쳣");

            // ģ��֧���ӳ�
            await Task.Delay(1500);

            payment.Status = PaymentStatus.Paid; // ��֧��
            payment.PaidAt = DateTime.UtcNow;

            // ����Ż�ȯ��ʹ��
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

            // ����ԤԼ״̬Ϊ��ȷ��
            payment.Appointment.Status = AppointmentStatus.Confirmed;

            await _db.SaveChangesAsync();

            // ����֧���ɹ��¼�������֪ͨ
            await _messageBus.PublishAsync("payment.completed", new PaymentCompletedEvent(payment.Id, payment.AppointmentId, userId, payment.FinalAmount));

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
            ?? throw new Exception("֧����������");

        if (payment.Status != PaymentStatus.Paid) throw new Exception("����֧���������˿�");

        payment.Status = PaymentStatus.Refunded; // ���˿�
        payment.Appointment.Status = AppointmentStatus.Cancelled; // ԤԼȡ��

        // �˻��Ż�ȯ
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
            .Select(p => new PaymentDto(p.Id, p.AppointmentId, p.UserId, p.Amount, p.DiscountAmount, p.FinalAmount, p.CouponId, (int)p.Status, (int)p.PayMethod, p.PaidAt, p.CreatedAt, p.Appointment.Service.Name, p.Appointment.Pet.Name))
            .ToListAsync();
    }

    public async Task<List<PaymentDto>> GetAllPaymentsAsync()
    {
        return await _db.Payments
            .Include(p => p.Appointment).ThenInclude(a => a.Service)
            .Include(p => p.Appointment).ThenInclude(a => a.Pet)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentDto(p.Id, p.AppointmentId, p.UserId, p.Amount, p.DiscountAmount, p.FinalAmount, p.CouponId, (int)p.Status, (int)p.PayMethod, p.PaidAt, p.CreatedAt, p.Appointment.Service.Name, p.Appointment.Pet.Name))
            .ToListAsync();
    }

    private static PaymentDto MapPayment(Payment p, string serviceName, string petName)
    {
        return new PaymentDto(p.Id, p.AppointmentId, p.UserId, p.Amount, p.DiscountAmount, p.FinalAmount, p.CouponId, (int)p.Status, (int)p.PayMethod, p.PaidAt, p.CreatedAt, serviceName, petName);
    }
}
