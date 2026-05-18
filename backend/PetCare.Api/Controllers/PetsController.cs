using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PetCare.Core.Dtos;
using PetCare.Core.Interfaces;

namespace PetCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetsController : ControllerBase
{
    private readonly IPetService _petService;
    public PetsController(IPetService petService) { _petService = petService; }

    [HttpGet]
    public async Task<IActionResult> GetMyPets()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await _petService.GetUserPetsAsync(userId);
        return Ok(new { data = list });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePetRequest req)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var pet = await _petService.CreatePetAsync(userId, req);
        return Ok(new { data = pet });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePetRequest req)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var pet = await _petService.UpdatePetAsync(userId, id, req);
        return Ok(new { data = pet });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _petService.DeletePetAsync(userId, id);
        return Ok(new { message = "已删除" });
    }
}
