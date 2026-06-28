using Microsoft.AspNetCore.Mvc;
using PetCare.Core.Interfaces;
namespace PetCare.Api.Controllers;
/// <summary>服务项目控制器，公开接口，无需认证</summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IAdminService _adminService;
    public ServicesController(IAdminService adminService) { _adminService = adminService; }
    /// <summary>获取服务列表，支持按类别和宠物类型筛选</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category, [FromQuery] string? petType)
    {
        var list = await _adminService.GetServicesAsync(category, petType);
        return Ok(new { data = list });
    }
    /// <summary>获取服务详情</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var s = await _adminService.GetServiceAsync(id);
        if (s == null) return NotFound();
        return Ok(new { data = s });
    }
}
