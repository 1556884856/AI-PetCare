using Microsoft.EntityFrameworkCore;
using PetCare.Core.Dtos;
using PetCare.Core.Entities;
using PetCare.Core.Events;
using PetCare.Core.Interfaces;
using PetCare.Data;
using PetCare.Core.Enums;
using PetCare.Service.Redis;
namespace PetCare.Service;
/// <summary>
/// 优惠券服务实现，包含管理端和用户端功能。
/// 使用 Redis 存储优惠券库存并控制并发领取（分布式锁 + 原子递减）。
/// </summary>
public class CouponService : ICouponService
{
    private readonly PetCareDbContext _db;
    private readonly RedisConnection _redis;
    private readonly IMessageBus _messageBus;
    public CouponService(PetCareDbContext db, RedisConnection redis, IMessageBus messageBus) { _db = db; _redis = redis; _messageBus = messageBus; }
    /// <summary>创建优惠券模板，同时初始化 Redis 库存</summary>
    public async Task<CouponDto> CreateCouponAsync(CreateCouponRequest r)
    {
        var coupon = new Coupon { Name = r.Name, Type = r.Type, Value = r.Value, MinOrderAmount = r.MinOrderAmount, ValidFrom = r.ValidFrom, ValidTo = r.ValidTo, TotalQuantity = r.TotalQuantity };
        _db.Coupons.Add(coupon);
        await _db.SaveChangesAsync();
        await _redis.Database.StringSetAsync($"coupon:stock:{coupon.Id}", coupon.TotalQuantity);
        return MapCoupon(coupon);
    }
    public async Task<List<CouponDto>> GetAllCouponsAsync()
    {
        var coupons = await _db.Coupons.OrderByDescending(c => c.CreatedAt).ToListAsync();
        return coupons.Select(c => { var used = _db.UserCoupons.Count(uc => uc.CouponId == c.Id); return new CouponDto(c.Id, c.Name, (int)c.Type, c.Value, c.MinOrderAmount, c.ValidFrom, c.ValidTo, c.TotalQuantity, used, c.IsActive, c.CreatedAt); }).ToList();
    }
    /// <summary>更新优惠券模板，同步 Redis 库存并清空相关缓存</summary>
    public async Task<CouponDto> UpdateCouponAsync(int id, UpdateCouponRequest r)
    {
        var c = await _db.Coupons.FindAsync(id) ?? throw new Exception("优惠券不存在");
        c.Name = r.Name; c.Type = r.Type; c.Value = r.Value; c.MinOrderAmount = r.MinOrderAmount; c.ValidFrom = r.ValidFrom; c.ValidTo = r.ValidTo; c.TotalQuantity = r.TotalQuantity; c.IsActive = r.IsActive;
        await _db.SaveChangesAsync();
        await _redis.Database.StringSetAsync($"coupon:stock:{c.Id}", c.TotalQuantity);
        await _redis.Database.KeyDeleteAsync("coupon:available");
        var used = await _db.UserCoupons.CountAsync(uc => uc.CouponId == c.Id);
        return new CouponDto(c.Id, c.Name, (int)c.Type, c.Value, c.MinOrderAmount, c.ValidFrom, c.ValidTo, c.TotalQuantity, used, c.IsActive, c.CreatedAt);
    }
    /// <summary>删除优惠券，清除 Redis 缓存</summary>
    public async Task DeleteCouponAsync(int id)
    {
        var c = await _db.Coupons.FindAsync(id) ?? throw new Exception("优惠券不存在");
        _db.Coupons.Remove(c); await _db.SaveChangesAsync();
        await _redis.Database.KeyDeleteAsync($"coupon:stock:{id}");
        await _redis.Database.KeyDeleteAsync("coupon:available");
    }
    /// <summary>批量分发优惠券（给指定用户每人一张），发布异步通知</summary>
    public async Task DistributeCouponAsync(int couponId, int[] userIds)
    {
        var coupon = await _db.Coupons.FindAsync(couponId) ?? throw new Exception("优惠券不存在");
        foreach (var userId in userIds)
        {
            var exists = await _db.UserCoupons.AnyAsync(uc => uc.UserId == userId && uc.CouponId == couponId);
            if (exists) continue;
            _db.UserCoupons.Add(new UserCoupon { UserId = userId, CouponId = couponId });
        }
        await _db.SaveChangesAsync();
        await _messageBus.PublishAsync("coupon.received", new CouponReceivedEvent(0, coupon.Name, coupon.Value));
    }
    /// <summary>获取用户可领取的优惠券列表（有效期内、未领完的）</summary>
    public async Task<List<CouponDto>> GetAvailableCouponsAsync(int userId)
    {
        var now = DateTime.UtcNow;
        var coupons = await _db.Coupons.Where(c => c.IsActive && c.ValidFrom <= now && c.ValidTo >= now).OrderByDescending(c => c.CreatedAt).ToListAsync();
        var result = new List<CouponDto>();
        foreach (var c in coupons)
        {
            var used = await _db.UserCoupons.CountAsync(uc => uc.CouponId == c.Id);
            var stockKey = $"coupon:stock:{c.Id}";
            var stock = await _redis.Database.StringGetAsync(stockKey);
            result.Add(new CouponDto(c.Id, c.Name, (int)c.Type, c.Value, c.MinOrderAmount, c.ValidFrom, c.ValidTo, c.TotalQuantity, used, c.IsActive, c.CreatedAt));
        }
        return result;
    }
    /// <summary>获取用户持有的优惠券，按状态筛选（unused/used/expired）</summary>
    public async Task<List<UserCouponDto>> GetUserCouponsAsync(int userId, string? status)
    {
        var query = _db.UserCoupons.Where(uc => uc.UserId == userId).Include(uc => uc.Coupon).AsQueryable();
        var now = DateTime.UtcNow;
        if (status == "unused") query = query.Where(uc => !uc.IsUsed && uc.Coupon.ValidTo >= now);
        else if (status == "used") query = query.Where(uc => uc.IsUsed);
        else if (status == "expired") query = query.Where(uc => !uc.IsUsed && uc.Coupon.ValidTo < now);
        return await query.OrderByDescending(uc => uc.CreatedAt).Select(uc => new UserCouponDto(uc.Id, uc.CouponId, uc.Coupon.Name, (int)uc.Coupon.Type, uc.Coupon.Value, uc.Coupon.MinOrderAmount, uc.Coupon.ValidFrom, uc.Coupon.ValidTo, uc.IsUsed, uc.UsedAt, uc.CreatedAt)).ToListAsync();
    }
    /// <summary>
    /// 用户领取优惠券。使用 Redis 分布式锁防止重复领取，
    /// 通过 Redis 原子递减控制库存，库存不足时回滚。
    /// </summary>
    public async Task ClaimCouponAsync(int userId, int couponId)
    {
        // Redis 分布式锁：防止并发重复领取
        var lockKey = $"coupon:claim:{userId}:{couponId}";
        var acquired = await _redis.Database.StringSetAsync(lockKey, "1", TimeSpan.FromSeconds(60), StackExchange.Redis.When.NotExists);
        if (!acquired) throw new Exception("操作频繁，请稍后");
        try
        {
            var coupon = await _db.Coupons.FindAsync(couponId) ?? throw new Exception("优惠券不存在");
            if (!coupon.IsActive || coupon.ValidFrom > DateTime.UtcNow || coupon.ValidTo < DateTime.UtcNow) throw new Exception("优惠券不在有效期内");
            var already = await _db.UserCoupons.AnyAsync(uc => uc.UserId == userId && uc.CouponId == couponId);
            if (already) throw new Exception("已领取过该优惠券");
            // Redis 库存扣减
            var stockKey = $"coupon:stock:{couponId}";
            var stock = await _redis.Database.StringGetAsync(stockKey);
            if (!stock.HasValue) await _redis.Database.StringSetAsync(stockKey, coupon.TotalQuantity);
            var remaining = await _redis.Database.StringDecrementAsync(stockKey);
            if (remaining < 0) { await _redis.Database.StringIncrementAsync(stockKey); throw new Exception("优惠券已抢光"); }
            _db.UserCoupons.Add(new UserCoupon { UserId = userId, CouponId = couponId });
            await _db.SaveChangesAsync();
            await _redis.Database.KeyDeleteAsync($"coupon:my:{userId}");
        }
        finally { await _redis.Database.KeyDeleteAsync(lockKey); }
    }
    private static CouponDto MapCoupon(Coupon c) => new CouponDto(c.Id, c.Name, (int)c.Type, c.Value, c.MinOrderAmount, c.ValidFrom, c.ValidTo, c.TotalQuantity, 0, c.IsActive, c.CreatedAt);
}
