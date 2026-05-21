namespace PetCare.Core.Entities;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    /// <summary>0=预约提醒 1=状态变更 2=促销活动 3=系统通知</summary>
    public int Type { get; set; }
    /// <summary>关联ID（如预约ID），可为空</summary>
    public int? RelatedId { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
