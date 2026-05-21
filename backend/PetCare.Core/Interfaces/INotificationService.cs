using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDto>> GetUserNotificationsAsync(int userId, int page, int pageSize);
    Task<int> GetUnreadCountAsync(int userId);
    Task MarkAsReadAsync(int userId, int notificationId);
    Task MarkAllAsReadAsync(int userId);
    Task DeleteNotificationAsync(int userId, int notificationId);
    Task SendBulkNotificationAsync(int[]? userIds, string title, string content, int type);
    Task CreateNotificationAsync(int userId, string title, string content, int type, int? relatedId);
}
