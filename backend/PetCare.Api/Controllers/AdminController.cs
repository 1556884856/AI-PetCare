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
    private readonly INotificationService _notificationService;
    private readonly ICouponService _couponService;
    private readonly IPaymentService _paymentService;
    public AdminController(IAdminService adminService, IPetService petService, INotificationService notificationService, ICouponService couponService, IPaymentService paymentService)
    {
        _adminService = adminService;
        _petService = petService;
        _notificationService = notificationService;
        _couponService = couponService;
        _paymentService = paymentService;
    }

    // ===== Dashboard =====
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var d = await _adminService.GetDashboardAsync();
        return Ok(new { data = d });
    }

    // ===== Appointments =====
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

    // ===== Services =====
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

    // ===== Customers =====
    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers()
    {
        var list = await _adminService.GetCustomersAsync();
        return Ok(new { data = list });
    }

    // ===== Pets =====
    [HttpGet("pets")]
    public async Task<IActionResult> GetAllPets()
    {
        var list = await _petService.GetAllPetsAsync();
        return Ok(new { data = list });
    }

    // ===== Notifications =====
    [HttpPost("notifications/send")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest req)
    {
        await _notificationService.SendBulkNotificationAsync(req.UserIds, req.Title, req.Content, req.Type);
        return Ok(new { message = "发送成功" });
    }

    // ===== Coupons =====
    [HttpPost("coupons")]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponRequest req)
    {
        var c = await _couponService.CreateCouponAsync(req);
        return Ok(new { data = c });
    }

    [HttpGet("coupons")]
    public async Task<IActionResult> GetAllCoupons()
    {
        var list = await _couponService.GetAllCouponsAsync();
        return Ok(new { data = list });
    }

    [HttpPut("coupons/{id}")]
    public async Task<IActionResult> UpdateCoupon(int id, [FromBody] UpdateCouponRequest req)
    {
        var c = await _couponService.UpdateCouponAsync(id, req);
        return Ok(new { data = c });
    }

    [HttpDelete("coupons/{id}")]
    public async Task<IActionResult> DeleteCoupon(int id)
    {
        await _couponService.DeleteCouponAsync(id);
        return Ok(new { message = "已删除" });
    }

    [HttpPost("coupons/{id}/distribute")]
    public async Task<IActionResult> DistributeCoupon(int id, [FromBody] DistributeCouponRequest req)
    {
        await _couponService.DistributeCouponAsync(id, req.UserIds);
        return Ok(new { message = "派发成功" });
    }

    // ===== Payments =====
    [HttpGet("payments")]
    public async Task<IActionResult> GetAllPayments()
    {
        var list = await _paymentService.GetAllPaymentsAsync();
        return Ok(new { data = list });
    }

    [HttpPost("payments/{id}/refund")]
    public async Task<IActionResult> RefundPayment(int id)
    {
        var p = await _paymentService.RefundAsync(id);
        return Ok(new { data = p });
    }
}
