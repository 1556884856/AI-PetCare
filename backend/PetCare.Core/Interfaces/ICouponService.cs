using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

public interface ICouponService
{
    // Admin
    Task<CouponDto> CreateCouponAsync(CreateCouponRequest request);
    Task<List<CouponDto>> GetAllCouponsAsync();
    Task<CouponDto> UpdateCouponAsync(int id, UpdateCouponRequest request);
    Task DeleteCouponAsync(int id);
    Task DistributeCouponAsync(int couponId, int[] userIds);

    // User
    Task<List<CouponDto>> GetAvailableCouponsAsync(int userId);
    Task<List<UserCouponDto>> GetUserCouponsAsync(int userId, string? status);
    Task ClaimCouponAsync(int userId, int couponId);
}
