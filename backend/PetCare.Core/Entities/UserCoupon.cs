namespace PetCare.Core.Entities;

/// <summary>
/// 用户优惠券关联实体，记录用户领取优惠券后的持有状态。
/// 包含是否已使用、使用时间等信息，通过 Redis 控制库存和领取操作。
/// </summary>
public class UserCoupon
{
    /// <summary>记录ID</summary>
    public int Id { get; set; }
    
    /// <summary>用户ID</summary>
    public int UserId { get; set; }
    
    /// <summary>优惠券ID</summary>
    public int CouponId { get; set; }
    
    /// <summary>是否已使用</summary>
    public bool IsUsed { get; set; } = false;
    
    /// <summary>使用时间</summary>
    public DateTime? UsedAt { get; set; }
    
    /// <summary>领取时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>用户导航属性</summary>
    public User User { get; set; } = null!;
    
    /// <summary>优惠券导航属性</summary>
    public Coupon Coupon { get; set; } = null!;
}
