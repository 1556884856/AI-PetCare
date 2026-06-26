using PetCare.Core.Enums;
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
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public PaymentMethod PayMethod { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Appointment Appointment { get; set; } = null!;
    public Coupon? Coupon { get; set; }
}
