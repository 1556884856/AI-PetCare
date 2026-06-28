using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

/// <summary>通知服务接口，管理用户通知的查询、标记已读和发送</summary>
public interface INotificationService
{
    /// <summary>分页获取用户通知列表</summary>
    Task<List<NotificationDto>> GetUserNotificationsAsync(int userId, int page, int pageSize);
    
    /// <summary>获取未读通知数量（优先从 Redis 缓存读取）</summary>
    Task<int> GetUnreadCountAsync(int userId);
    
    /// <summary>标记单条通知为已读</summary>
    Task MarkAsReadAsync(int userId, int notificationId);
    
    /// <summary>全部标记为已读</summary>
    Task MarkAllAsReadAsync(int userId);
    
    /// <summary>删除单条通知</summary>
    Task DeleteNotificationAsync(int userId, int notificationId);
    
    /// <summary>批量发送通知（管理员群发用）</summary>
    Task SendBulkNotificationAsync(int[]? userIds, string title, string content, int type);
    
    /// <summary>为指定用户创建单条通知（供消息消费者等内部服务调用）</summary>
    Task CreateNotificationAsync(int userId, string title, string content, int type, int? relatedId);
}
