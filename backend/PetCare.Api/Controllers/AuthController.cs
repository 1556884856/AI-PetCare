using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PetCare.Core.Dtos;
using PetCare.Core.Interfaces;
namespace PetCare.Api.Controllers;
/// <summary>认证控制器，处理登录、注册和个人信息管理</summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    public AuthController(IUserService userService) { _userService = userService; }
    /// <summary>发送手机验证码</summary>
    [HttpPost("send-code")]
    public async Task<IActionResult> SendCode([FromBody] SendCodeRequest req)
    {
        await _userService.SendCodeAsync(req.Phone);
        return Ok(new { message = "验证码已发送" });
    }
    /// <summary>手机号验证码登录（首次登录自动注册）</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var result = await _userService.LoginAsync(req.Phone, req.Code);
        return Ok(new { data = result });
    }
    /// <summary>获取当前登录用户信息（需要认证）</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userService.GetUserAsync(userId);
        return Ok(new { data = user });
    }
    /// <summary>更新个人信息</summary>
    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userService.UpdateProfileAsync(userId, req);
        return Ok(new { data = user });
    }
}
