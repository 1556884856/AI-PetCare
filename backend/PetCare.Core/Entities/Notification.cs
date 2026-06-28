namespace PetCare.Core.Entities;

/// <summary>
/// 通知实体，存储用户的消息通知。
/// 支持多种类型：0=预约提醒, 1=状态变更, 2=促销活动, 3=系统通知。
/// 可关联具体的业务记录（如预约ID），方便点击跳转。
/// </summary>
public class Notification
{
    /// <summary>通知ID</summary>
    public int Id { get; set; }
    
    /// <summary>接收用户ID</summary>
    public int UserId { get; set; }
    
    /// <summary>通知标题</summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>通知内容</summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>通知类型：0=预约提醒, 1=状态变更, 2=促销活动, 3=系统通知</summary>
    public int Type { get; set; }
    
    /// <summary>关联业务ID（如预约ID），点击跳转用</summary>
    public int? RelatedId { get; set; }
    
    /// <summary>是否已读</summary>
    public bool IsRead { get; set; } = false;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>接收用户导航属性</summary>
    public User User { get; set; } = null!;
}
