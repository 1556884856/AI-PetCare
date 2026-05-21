using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PetCare.Core.Dtos;
using PetCare.Core.Interfaces;

namespace PetCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    public PaymentsController(IPaymentService paymentService) { _paymentService = paymentService; }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequest req)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var payment = await _paymentService.CreatePaymentAsync(userId, req);
        return Ok(new { data = payment });
    }

    [HttpPut("{id}/pay")]
    public async Task<IActionResult> MockPay(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var payment = await _paymentService.MockPayAsync(userId, id);
        return Ok(new { data = payment });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        var payment = await _paymentService.GetPaymentAsync(id);
        if (payment == null) return NotFound(new { message = "支付单不存在" });
        return Ok(new { data = payment });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyPayments()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await _paymentService.GetUserPaymentsAsync(userId);
        return Ok(new { data = list });
    }
}
