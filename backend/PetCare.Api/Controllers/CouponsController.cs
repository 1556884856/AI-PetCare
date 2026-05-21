using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PetCare.Core.Dtos;
using PetCare.Core.Interfaces;

namespace PetCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CouponsController : ControllerBase
{
    private readonly ICouponService _couponService;
    public CouponsController(ICouponService couponService) { _couponService = couponService; }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await _couponService.GetAvailableCouponsAsync(userId);
        return Ok(new { data = list });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyCoupons([FromQuery] string? status)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await _couponService.GetUserCouponsAsync(userId, status);
        return Ok(new { data = list });
    }

    [HttpPost("{id}/claim")]
    public async Task<IActionResult> Claim(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _couponService.ClaimCouponAsync(userId, id);
        return Ok(new { message = "领取成功" });
    }
}
