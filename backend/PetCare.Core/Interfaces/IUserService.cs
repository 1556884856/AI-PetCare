using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

/// <summary>用户服务接口，负责登录注册、个人信息管理</summary>
public interface IUserService
{
    /// <summary>手机号验证码登录（首次登录自动注册），返回 JWT Token 和用户信息</summary>
    Task<LoginResponse> LoginAsync(string phone, string code);
    
    /// <summary>发送短信验证码</summary>
    Task SendCodeAsync(string phone);
    
    /// <summary>获取当前用户信息</summary>
    Task<UserDto> GetUserAsync(int userId);
    
    /// <summary>更新用户个人信息</summary>
    Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request);
}
