using Microsoft.EntityFrameworkCore;
using PetCare.Core.Dtos;
using PetCare.Core.Entities;
using PetCare.Core.Interfaces;
using PetCare.Data;
namespace PetCare.Service;
/// <summary>
/// 宠物服务实现，管理用户宠物的增删改查。
/// 删除为软删除（IsDeleted=true），通过全局查询过滤器隐藏。
/// </summary>
public class PetService : IPetService
{
    private readonly PetCareDbContext _db;
    public PetService(PetCareDbContext db) { _db = db; }
    /// <summary>获取当前用户的所有宠物（未删除），按创建时间倒序</summary>
    public async Task<List<PetDto>> GetUserPetsAsync(int userId)
    {
        var pets = await _db.Pets.Where(p => p.UserId == userId).OrderByDescending(p => p.CreatedAt).ToListAsync();
        return pets.Select(Map).ToList();
    }
    /// <summary>创建新宠物</summary>
    public async Task<PetDto> CreatePetAsync(int userId, CreatePetRequest r)
    {
        var pet = new Pet { UserId = userId, Name = r.Name, Type = r.Type, Breed = r.Breed, Age = r.Age, Weight = r.Weight, Notes = r.Notes };
        _db.Pets.Add(pet);
        await _db.SaveChangesAsync();
        return Map(pet);
    }
    /// <summary>更新宠物信息，校验归属权</summary>
    public async Task<PetDto> UpdatePetAsync(int userId, int petId, UpdatePetRequest r)
    {
        var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == petId && p.UserId == userId) ?? throw new Exception("宠物不存在");
        pet.Name = r.Name; pet.Type = r.Type; pet.Breed = r.Breed; pet.Age = r.Age; pet.Weight = r.Weight; pet.Notes = r.Notes;
        await _db.SaveChangesAsync();
        return Map(pet);
    }
    /// <summary>软删除宠物（设置 IsDeleted=true）</summary>
    public async Task DeletePetAsync(int userId, int petId)
    {
        var pet = await _db.Pets.FirstOrDefaultAsync(p => p.Id == petId && p.UserId == userId) ?? throw new Exception("宠物不存在");
        pet.IsDeleted = true;
        await _db.SaveChangesAsync();
    }
    /// <summary>管理员功能：获取所有宠物（含已删除的），绕过全局过滤器</summary>
    public async Task<List<PetDto>> GetAllPetsAsync()
    {
        var pets = await _db.Pets.IgnoreQueryFilters().Include(p => p.User).OrderByDescending(p => p.CreatedAt).ToListAsync();
        return pets.Select(p => new PetDto(p.Id, p.Name, p.Type, p.Breed, p.Age, p.Weight, p.Notes, p.CreatedAt)).ToList();
    }
    private static PetDto Map(Pet p) => new(p.Id, p.Name, p.Type, p.Breed, p.Age, p.Weight, p.Notes, p.CreatedAt);
}
