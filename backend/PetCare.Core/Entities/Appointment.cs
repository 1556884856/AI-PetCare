using PetCare.Core.Enums;
namespace PetCare.Core.Entities;

/// <summary>
/// 预约实体，记录用户为宠物预约某项服务的完整信息。
/// 包含预约日期、时段、状态等核心字段，状态流转由管理员通过 AdminService 操作。
/// </summary>
public class Appointment
{
    /// <summary>预约ID</summary>
    public int Id { get; set; }
    
    /// <summary>预约用户ID</summary>
    public int UserId { get; set; }
    
    /// <summary>预约的宠物ID</summary>
    public int PetId { get; set; }
    
    /// <summary>预约的服务ID</summary>
    public int ServiceId { get; set; }
    
    /// <summary>预约日期</summary>
    public DateTime AppointmentDate { get; set; }
    
    /// <summary>预约时段（如 "09:00-10:00"）</summary>
    public string TimeSlot { get; set; } = string.Empty;
    
    /// <summary>预约状态（Pending/Confirmed/Completed/Cancelled）</summary>
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    
    /// <summary>备注</summary>
    public string Notes { get; set; } = string.Empty;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>预约用户导航属性</summary>
    public User User { get; set; } = null!;
    
    /// <summary>预约宠物导航属性</summary>
    public Pet Pet { get; set; } = null!;
    
    /// <summary>预约服务导航属性</summary>
    public Service Service { get; set; } = null!;
    
    /// <summary>关联的评价（一对零或一）</summary>
    public Review? Review { get; set; }
}
