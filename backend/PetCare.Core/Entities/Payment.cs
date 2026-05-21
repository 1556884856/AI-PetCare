namespace PetCare.Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public int? CouponId { get; set; }
    /// <summary>0=待支付 1=已支付 2=已退款</summary>
    public int Status { get; set; } = 0;
    /// <summary>0=微信 1=支付宝 2=到店付</summary>
    public int PayMethod { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Appointment Appointment { get; set; } = null!;
    public Coupon? Coupon { get; set; }
}
