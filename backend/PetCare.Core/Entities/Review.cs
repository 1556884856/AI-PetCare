namespace PetCare.Core.Entities;

/// <summary>
/// 评价实体，用户完成服务后可对预约进行评分和留言。
/// 每个预约最多有一条评价（一对一关系）。
/// </summary>
public class Review
{
    /// <summary>评价ID</summary>
    public int Id { get; set; }
    
    /// <summary>评价用户ID</summary>
    public int UserId { get; set; }
    
    /// <summary>关联的预约ID</summary>
    public int AppointmentId { get; set; }
    
    /// <summary>评分（1-5星）</summary>
    public int Rating { get; set; }
    
    /// <summary>评价内容</summary>
    public string Comment { get; set; } = string.Empty;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>评价用户导航属性</summary>
    public User User { get; set; } = null!;
    
    /// <summary>关联的预约导航属性</summary>
    public Appointment Appointment { get; set; } = null!;
}
