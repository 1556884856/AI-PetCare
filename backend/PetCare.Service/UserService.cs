using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetCare.Core.Dtos;
using PetCare.Core.Entities;
using PetCare.Core.Enums;
using PetCare.Core.Interfaces;
using PetCare.Data;
using Microsoft.Extensions.Configuration;
namespace PetCare.Service;
/// <summary>
/// 用户服务实现，处理登录注册、JWT Token 生成和个人信息管理。
/// 首次登录自动注册，开发环境验证码固定为 1234。
/// </summary>
public class UserService : IUserService
{
    private readonly PetCareDbContext _db;
    private readonly IConfiguration _config;
    public UserService(PetCareDbContext db, IConfiguration config) { _db = db; _config = config; }
    /// <summary>发送短信验证码（开发环境占位实现）</summary>
    public Task SendCodeAsync(string phone) { return Task.CompletedTask; }
    /// <summary>手机号验证码登录。首次登录自动创建用户账号，返回 JWT Token 和用户信息。</summary>
    public async Task<LoginResponse> LoginAsync(string phone, string code)
    {
        // 开发环境：验证码固定为 1234
        if (code != "1234") throw new Exception("验证码错误");
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Phone == phone);
        if (user == null)
        {
            // 首次登录：自动注册，昵称取手机号后四位
            user = new User { Phone = phone, Nickname = "用户" + phone[^4..], Role = UserRole.Customer, CreatedAt = DateTime.UtcNow };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
        var token = GenerateJwt(user);
        return new LoginResponse(token, Map(user));
    }
    /// <summary>获取用户信息，不存在则抛异常</summary>
    public async Task<UserDto> GetUserAsync(int userId)
    {
        var user = await _db.Users.FindAsync(userId) ?? throw new Exception("用户不存在");
        return Map(user);
    }
    /// <summary>更新用户昵称</summary>
    public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _db.Users.FindAsync(userId) ?? throw new Exception("用户不存在");
        if (request.Nickname != null) user.Nickname = request.Nickname;
        await _db.SaveChangesAsync();
        return Map(user);
    }
    private static UserDto Map(User u) => new(u.Id, u.Phone, u.Nickname, u.AvatarUrl, (int)u.Role);
    /// <summary>生成 JWT Token，包含用户ID和角色声明，有效期7天</summary>
    private string GenerateJwt(User u)
    {
        var jwtKey = _config["Jwt:Key"] ?? "PetCareSecretKey2026ForJwtTokenGeneration!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, u.Id.ToString()),
            new Claim(ClaimTypes.Role, u.Role.ToString())
        };
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddDays(7), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
