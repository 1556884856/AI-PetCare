using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

/// <summary>优惠券服务接口，包含管理端和用户端功能</summary>
public interface ICouponService
{
    // ===== 管理端 =====
    
    /// <summary>创建优惠券模板</summary>
    Task<CouponDto> CreateCouponAsync(CreateCouponRequest request);
    
    /// <summary>获取所有优惠券模板列表</summary>
    Task<List<CouponDto>> GetAllCouponsAsync();
    
    /// <summary>更新优惠券模板</summary>
    Task<CouponDto> UpdateCouponAsync(int id, UpdateCouponRequest request);
    
    /// <summary>删除优惠券模板</summary>
    Task DeleteCouponAsync(int id);
    
    /// <summary>批量分发优惠券给指定用户</summary>
    Task DistributeCouponAsync(int couponId, int[] userIds);

    // ===== 用户端 =====
    
    /// <summary>获取用户可领取的优惠券列表</summary>
    Task<List<CouponDto>> GetAvailableCouponsAsync(int userId);
    
    /// <summary>获取用户持有的优惠券，可按状态筛选（unused/used/expired）</summary>
    Task<List<UserCouponDto>> GetUserCouponsAsync(int userId, string? status);
    
    /// <summary>用户领取优惠券（通过 Redis 控制库存和并发）</summary>
    Task ClaimCouponAsync(int userId, int couponId);
}
