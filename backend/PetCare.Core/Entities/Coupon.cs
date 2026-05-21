namespace PetCare.Core.Entities;

public class Coupon
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    /// <summary>0=满减 1=折扣</summary>
    public int Type { get; set; }
    /// <summary>面值：满减填金额，折扣填小数（如0.85表示85折）</summary>
    public decimal Value { get; set; }
    /// <summary>最低消费金额</summary>
    public decimal MinOrderAmount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int TotalQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
}
