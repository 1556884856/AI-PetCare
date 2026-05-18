using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

public interface IUserService
{
    Task<LoginResponse> LoginAsync(string phone, string code);
    Task SendCodeAsync(string phone);
    Task<UserDto> GetUserAsync(int userId);
    Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileRequest request);
}
