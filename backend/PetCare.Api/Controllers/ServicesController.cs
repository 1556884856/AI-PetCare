using Microsoft.AspNetCore.Mvc;
using PetCare.Core.Interfaces;

namespace PetCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IAdminService _adminService;
    public ServicesController(IAdminService adminService) { _adminService = adminService; }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category, [FromQuery] string? petType)
    {
        var list = await _adminService.GetServicesAsync(category, petType);
        return Ok(new { data = list });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var s = await _adminService.GetServiceAsync(id);
        if (s == null) return NotFound();
        return Ok(new { data = s });
    }
}
