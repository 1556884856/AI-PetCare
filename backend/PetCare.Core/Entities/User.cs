using PetCare.Core.Enums;
namespace PetCare.Core.Entities;

/// <summary>
/// 用户实体，包含用户的基本信息和关联的导航属性。
/// 手机号作为用户唯一标识，首次登录时自动注册。
/// </summary>
public class User
{
    /// <summary>用户ID</summary>
    public int Id { get; set; }
    
    /// <summary>手机号（唯一标识）</summary>
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>用户昵称</summary>
    public string Nickname { get; set; } = string.Empty;
    
    /// <summary>头像URL</summary>
    public string AvatarUrl { get; set; } = string.Empty;
    
    /// <summary>用户角色，默认为普通客户</summary>
    public UserRole Role { get; set; } = UserRole.Customer;
    
    /// <summary>账号创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ===== 导航属性（EF Core 关系映射） =====
    
    /// <summary>用户拥有的宠物列表</summary>
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    
    /// <summary>用户的预约记录</summary>
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    
    /// <summary>用户发表的评价</summary>
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    
    /// <summary>用户收到的通知</summary>
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    
    /// <summary>用户领取的优惠券</summary>
    public ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
    
    /// <summary>用户的支付记录</summary>
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
