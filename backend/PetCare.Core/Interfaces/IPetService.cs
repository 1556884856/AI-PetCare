using PetCare.Core.Dtos;

namespace PetCare.Core.Interfaces;

public interface IPetService
{
    Task<List<PetDto>> GetUserPetsAsync(int userId);
    Task<PetDto> CreatePetAsync(int userId, CreatePetRequest request);
    Task<PetDto> UpdatePetAsync(int userId, int petId, UpdatePetRequest request);
    Task DeletePetAsync(int userId, int petId);
    Task<List<PetDto>> GetAllPetsAsync();
}
