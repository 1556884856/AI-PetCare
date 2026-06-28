using Microsoft.EntityFrameworkCore;
using PetCare.Core.Dtos;
using PetCare.Core.Entities;
using PetCare.Core.Enums;
using PetCare.Core.Interfaces;
using PetCare.Data;
using PetCare.Service.Redis;
namespace PetCare.Service;
/// <summary>
/// 通知服务实现，管理用户通知的查询、已读标记和发送。
/// 未读数量使用 Redis 缓存，5分钟过期，修改后手动失效。
/// </summary>
public class NotificationService : INotificationService
{
    private readonly PetCareDbContext _db;
    private readonly RedisConnection _redis;
    public NotificationService(PetCareDbContext db, RedisConnection redis) { _db = db; _redis = redis; }
    /// <summary>分页获取用户通知列表，按创建时间倒序</summary>
    public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId, int page, int pageSize)
    {
        return await _db.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).Select(n => new NotificationDto(n.Id, n.Title, n.Content, n.Type, n.RelatedId, n.IsRead, n.CreatedAt)).ToListAsync();
    }
    /// <summary>获取未读通知数（优先从缓存读，5分钟过期）</summary>
    public async Task<int> GetUnreadCountAsync(int userId)
    {
        var key = $"notif:unread:{userId}";
        var cached = await _redis.Database.StringGetAsync(key);
        if (cached.HasValue && int.TryParse(cached, out var count)) return count;
        count = await _db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
        await _redis.Database.StringSetAsync(key, count, TimeSpan.FromMinutes(5));
        return count;
    }
    /// <summary>标记单条通知已读，更新缓存</summary>
    public async Task MarkAsReadAsync(int userId, int notificationId)
    {
        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId);
        if (n == null) throw new Exception("通知不存在");
        n.IsRead = true;
        await _db.SaveChangesAsync();
        await UpdateUnreadCacheAsync(userId);
    }
    /// <summary>全部标记已读</summary>
    public async Task MarkAllAsReadAsync(int userId)
    {
        var unread = await _db.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
        foreach (var n in unread) n.IsRead = true;
        await _db.SaveChangesAsync();
        await _redis.Database.StringSetAsync($"notif:unread:{userId}", 0);
    }
    /// <summary>删除单条通知</summary>
    public async Task DeleteNotificationAsync(int userId, int notificationId)
    {
        var n = await _db.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId);
        if (n == null) throw new Exception("通知不存在");
        _db.Notifications.Remove(n);
        await _db.SaveChangesAsync();
        await UpdateUnreadCacheAsync(userId);
    }
    /// <summary>批量发送通知（管理员群发），userIds为空则发送给所有用户</summary>
    public async Task SendBulkNotificationAsync(int[]? userIds, string title, string content, int type)
    {
        List<User> users;
        if (userIds == null || userIds.Length == 0) users = await _db.Users.ToListAsync();
        else users = await _db.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
        foreach (var user in users) _db.Notifications.Add(new Notification { UserId = user.Id, Title = title, Content = content, Type = type });
        await _db.SaveChangesAsync();
        foreach (var user in users) await _redis.Database.KeyDeleteAsync($"notif:unread:{user.Id}");
    }
    /// <summary>为指定用户创建单条通知（供消息消费者调用）</summary>
    public async Task CreateNotificationAsync(int userId, string title, string content, int type, int? relatedId)
    {
        _db.Notifications.Add(new Notification { UserId = userId, Title = title, Content = content, Type = type, RelatedId = relatedId });
        await _db.SaveChangesAsync();
        await _redis.Database.KeyDeleteAsync($"notif:unread:{userId}");
    }
    private async Task UpdateUnreadCacheAsync(int userId)
    {
        var count = await _db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
        await _redis.Database.StringSetAsync($"notif:unread:{userId}", count, TimeSpan.FromMinutes(5));
    }
}
