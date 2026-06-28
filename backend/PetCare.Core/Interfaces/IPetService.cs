using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

/// <summary>宠物服务接口，管理用户的宠物信息</summary>
public interface IPetService
{
    /// <summary>获取当前用户的所有宠物</summary>
    Task<List<PetDto>> GetUserPetsAsync(int userId);
    
    /// <summary>创建新宠物</summary>
    Task<PetDto> CreatePetAsync(int userId, CreatePetRequest request);
    
    /// <summary>更新宠物信息</summary>
    Task<PetDto> UpdatePetAsync(int userId, int petId, UpdatePetRequest request);
    
    /// <summary>删除宠物（软删除）</summary>
    Task DeletePetAsync(int userId, int petId);
    
    /// <summary>获取所有宠物（管理员功能）</summary>
    Task<List<PetDto>> GetAllPetsAsync();
}
