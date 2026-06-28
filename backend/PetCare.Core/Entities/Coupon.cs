namespace PetCare.Core.Entities;

/// <summary>
/// 优惠券实体，定义优惠券模板（由管理员创建）。
/// 类型分满减券（Type=0，Value为金额）和折扣券（Type=1，Value为折扣率如0.85表示85折）。
/// </summary>
public class Coupon
{
    /// <summary>优惠券ID</summary>
    public int Id { get; set; }
    
    /// <summary>优惠券名称</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>类型：0=满减券（Value为金额），1=折扣券（Value为折扣率，如0.85=85折）</summary>
    public int Type { get; set; }
    
    /// <summary>面值/折扣值：满减券为金额，折扣券为小数（如0.85表示85折）</summary>
    public decimal Value { get; set; }
    
    /// <summary>最低消费金额（0表示无门槛）</summary>
    public decimal MinOrderAmount { get; set; }
    
    /// <summary>有效期起始</summary>
    public DateTime ValidFrom { get; set; }
    
    /// <summary>有效期截止</summary>
    public DateTime ValidTo { get; set; }
    
    /// <summary>发放总量</summary>
    public int TotalQuantity { get; set; }
    
    /// <summary>是否启用</summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>用户领取的优惠券记录</summary>
    public ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
}
