using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PetCare.Core.Dtos;
using PetCare.Core.Interfaces;
namespace PetCare.Api.Controllers;
/// <summary>宠物控制器，用户管理自己的宠物（需要认证）</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PetsController : ControllerBase
{
    private readonly IPetService _petService;
    public PetsController(IPetService petService) { _petService = petService; }
    /// <summary>获取当前用户的所有宠物</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyPets()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await _petService.GetUserPetsAsync(userId);
        return Ok(new { data = list });
    }
    /// <summary>创建新宠物</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePetRequest req)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var pet = await _petService.CreatePetAsync(userId, req);
        return Ok(new { data = pet });
    }
    /// <summary>更新宠物信息</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePetRequest req)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var pet = await _petService.UpdatePetAsync(userId, id, req);
        return Ok(new { data = pet });
    }
    /// <summary>删除宠物（软删除）</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _petService.DeletePetAsync(userId, id);
        return Ok(new { message = "已删除" });
    }
}
