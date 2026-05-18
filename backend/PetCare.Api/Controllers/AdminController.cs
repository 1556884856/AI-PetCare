using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.Core.Dtos;
using PetCare.Core.Interfaces;

namespace PetCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IPetService _petService;
    public AdminController(IAdminService adminService, IPetService petService) { _adminService = adminService; _petService = petService; }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var d = await _adminService.GetDashboardAsync();
        return Ok(new { data = d });
    }

    [HttpGet("appointments")]
    public async Task<IActionResult> GetAllAppointments([FromQuery] string? date, [FromQuery] int? status)
    {
        var list = await _adminService.GetAllAppointmentsAsync(date, status);
        return Ok(new { data = list });
    }

    [HttpPut("appointments/{id}/confirm")]
    public async Task<IActionResult> ConfirmAppointment(int id)
    {
        var a = await _adminService.ConfirmAppointmentAsync(id);
        return Ok(new { data = a });
    }

    [HttpPut("appointments/{id}/complete")]
    public async Task<IActionResult> CompleteAppointment(int id)
    {
        var a = await _adminService.CompleteAppointmentAsync(id);
        return Ok(new { data = a });
    }

    [HttpPut("appointments/{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        var a = await _adminService.CancelAppointmentAsync(id);
        return Ok(new { data = a });
    }

    [HttpPost("services")]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest req)
    {
        var s = await _adminService.CreateServiceAsync(req);
        return Ok(new { data = s });
    }

    [HttpPut("services/{id}")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceRequest req)
    {
        var s = await _adminService.UpdateServiceAsync(id, req);
        return Ok(new { data = s });
    }

    [HttpDelete("services/{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        await _adminService.DeleteServiceAsync(id);
        return Ok(new { message = "已删除" });
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers()
    {
        var list = await _adminService.GetCustomersAsync();
        return Ok(new { data = list });
    }

    [HttpGet("pets")]
    public async Task<IActionResult> GetAllPets()
    {
        var list = await _petService.GetAllPetsAsync();
        return Ok(new { data = list });
    }
}
