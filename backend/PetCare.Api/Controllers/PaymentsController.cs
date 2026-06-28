using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PetCare.Core.Dtos;
using PetCare.Core.Interfaces;
namespace PetCare.Api.Controllers;
/// <summary>支付控制器，用户创建和管理自己的支付单（需要认证）</summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    public PaymentsController(IPaymentService paymentService) { _paymentService = paymentService; }
    /// <summary>创建支付单</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequest req)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var payment = await _paymentService.CreatePaymentAsync(userId, req);
        return Ok(new { data = payment });
    }
    /// <summary>模拟支付</summary>
    [HttpPut("{id}/pay")]
    public async Task<IActionResult> MockPay(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var payment = await _paymentService.MockPayAsync(userId, id);
        return Ok(new { data = payment });
    }
    /// <summary>获取支付单详情</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        var payment = await _paymentService.GetPaymentAsync(id);
        if (payment == null) return NotFound(new { message = "支付单不存在" });
        return Ok(new { data = payment });
    }
    /// <summary>获取当前用户的支付记录</summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyPayments()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await _paymentService.GetUserPaymentsAsync(userId);
        return Ok(new { data = list });
    }
}
