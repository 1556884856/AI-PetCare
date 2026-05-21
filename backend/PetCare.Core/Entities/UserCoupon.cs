namespace PetCare.Core.Entities;

public class UserCoupon
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CouponId { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Coupon Coupon { get; set; } = null!;
}
