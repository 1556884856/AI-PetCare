using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.Core.Dtos;
using PetCare.Core.Interfaces;
namespace PetCare.Api.Controllers;
/// <summary>管理员控制器，汇聚仪表盘、预约、服务、客户、宠物、通知、优惠券、支付的后台管理功能（需要认证）</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IPetService _petService;
    private readonly INotificationService _notificationService;
    private readonly ICouponService _couponService;
    private readonly IPaymentService _paymentService;
    public AdminController(IAdminService adminService, IPetService petService, INotificationService notificationService, ICouponService couponService, IPaymentService paymentService)
    { _adminService = adminService; _petService = petService; _notificationService = notificationService; _couponService = couponService; _paymentService = paymentService; }
    // ===== 仪表盘 =====
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard() { var d = await _adminService.GetDashboardAsync(); return Ok(new { data = d }); }
    // ===== 预约管理 =====
    [HttpGet("appointments")]
    public async Task<IActionResult> GetAllAppointments([FromQuery] string? date, [FromQuery] int? status) { var list = await _adminService.GetAllAppointmentsAsync(date, status); return Ok(new { data = list }); }
    /// <summary>确认预约</summary>
    [HttpPut("appointments/{id}/confirm")]
    public async Task<IActionResult> ConfirmAppointment(int id) { var a = await _adminService.ConfirmAppointmentAsync(id); return Ok(new { data = a }); }
    /// <summary>完成预约</summary>
    [HttpPut("appointments/{id}/complete")]
    public async Task<IActionResult> CompleteAppointment(int id) { var a = await _adminService.CompleteAppointmentAsync(id); return Ok(new { data = a }); }
    /// <summary>取消预约（管理员）</summary>
    [HttpPut("appointments/{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(int id) { var a = await _adminService.CancelAppointmentAsync(id); return Ok(new { data = a }); }
    // ===== 服务管理 =====
    [HttpPost("services")]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest req) { var s = await _adminService.CreateServiceAsync(req); return Ok(new { data = s }); }
    [HttpPut("services/{id}")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceRequest req) { var s = await _adminService.UpdateServiceAsync(id, req); return Ok(new { data = s }); }
    [HttpDelete("services/{id}")]
    public async Task<IActionResult> DeleteService(int id) { await _adminService.DeleteServiceAsync(id); return Ok(new { message = "已删除" }); }
    // ===== 客户管理 =====
    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers() { var list = await _adminService.GetCustomersAsync(); return Ok(new { data = list }); }
    // ===== 宠物管理（管理员查看所有宠物）=====
    [HttpGet("pets")]
    public async Task<IActionResult> GetAllPets() { var list = await _petService.GetAllPetsAsync(); return Ok(new { data = list }); }
    // ===== 通知群发 =====
    [HttpPost("notifications/send")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest req) { await _notificationService.SendBulkNotificationAsync(req.UserIds, req.Title, req.Content, req.Type); return Ok(new { message = "发送成功" }); }
    // ===== 优惠券管理 =====
    [HttpPost("coupons")]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponRequest req) { var c = await _couponService.CreateCouponAsync(req); return Ok(new { data = c }); }
    [HttpGet("coupons")]
    public async Task<IActionResult> GetAllCoupons() { var list = await _couponService.GetAllCouponsAsync(); return Ok(new { data = list }); }
    [HttpPut("coupons/{id}")]
    public async Task<IActionResult> UpdateCoupon(int id, [FromBody] UpdateCouponRequest req) { var c = await _couponService.UpdateCouponAsync(id, req); return Ok(new { data = c }); }
    [HttpDelete("coupons/{id}")]
    public async Task<IActionResult> DeleteCoupon(int id) { await _couponService.DeleteCouponAsync(id); return Ok(new { message = "已删除" }); }
    /// <summary>批量分发优惠券</summary>
    [HttpPost("coupons/{id}/distribute")]
    public async Task<IActionResult> DistributeCoupon(int id, [FromBody] DistributeCouponRequest req) { await _couponService.DistributeCouponAsync(id, req.UserIds); return Ok(new { message = "派发成功" }); }
    // ===== 支付管理 =====
    [HttpGet("payments")]
    public async Task<IActionResult> GetAllPayments() { var list = await _paymentService.GetAllPaymentsAsync(); return Ok(new { data = list }); }
    /// <summary>退款</summary>
    [HttpPost("payments/{id}/refund")]
    public async Task<IActionResult> RefundPayment(int id) { var p = await _paymentService.RefundAsync(id); return Ok(new { data = p }); }
}
