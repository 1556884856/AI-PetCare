using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PetCare.Core.Dtos;
using PetCare.Core.Interfaces;
namespace PetCare.Api.Controllers;
/// <summary>优惠券控制器（用户端），处理领取和查询（需要认证）</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CouponsController : ControllerBase
{
    private readonly ICouponService _couponService;
    public CouponsController(ICouponService couponService) { _couponService = couponService; }
    /// <summary>获取可领取的优惠券列表</summary>
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await _couponService.GetAvailableCouponsAsync(userId);
        return Ok(new { data = list });
    }
    /// <summary>获取我持有的优惠券，可按状态筛选</summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyCoupons([FromQuery] string? status)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await _couponService.GetUserCouponsAsync(userId, status);
        return Ok(new { data = list });
    }
    /// <summary>领取优惠券</summary>
    [HttpPost("{id}/claim")]
    public async Task<IActionResult> Claim(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _couponService.ClaimCouponAsync(userId, id);
        return Ok(new { message = "领取成功" });
    }
}
