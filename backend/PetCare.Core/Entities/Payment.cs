using PetCare.Core.Enums;
namespace PetCare.Core.Entities;

/// <summary>
/// 支付实体，记录每笔支付信息的完整生命周期。
/// 包含原价、优惠金额、实付金额以及支付方式和状态。
/// 支持使用优惠券抵扣，关联优惠券记录。
/// </summary>
public class Payment
{
    /// <summary>支付单ID</summary>
    public int Id { get; set; }
    
    /// <summary>关联的预约ID</summary>
    public int AppointmentId { get; set; }
    
    /// <summary>支付用户ID</summary>
    public int UserId { get; set; }
    
    /// <summary>原价金额</summary>
    public decimal Amount { get; set; }
    
    /// <summary>优惠金额</summary>
    public decimal DiscountAmount { get; set; }
    
    /// <summary>实付金额（= Amount - DiscountAmount）</summary>
    public decimal FinalAmount { get; set; }
    
    /// <summary>使用的优惠券用户关联ID（可为空，表示未使用优惠券）</summary>
    public int? CouponId { get; set; }
    
    /// <summary>支付状态（Pending/Paid/Refunded）</summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    /// <summary>支付方式</summary>
    public PaymentMethod PayMethod { get; set; }
    
    /// <summary>支付时间</summary>
    public DateTime? PaidAt { get; set; }
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>支付用户导航属性</summary>
    public User User { get; set; } = null!;
    
    /// <summary>关联的预约导航属性</summary>
    public Appointment Appointment { get; set; } = null!;
    
    /// <summary>使用的优惠券导航属性</summary>
    public Coupon? Coupon { get; set; }
}
