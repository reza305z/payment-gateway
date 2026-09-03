using Microsoft.AspNetCore.Mvc;
using PaymentApi.DTOs.Payment;
using PaymentApi.Services;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("get-token")]
    public async Task<ActionResult<GetTokenResponse>> GetToken(
        GetTokenRequest request)
    {
        var response = await _paymentService.GetTokenAsync(request);
        return Ok(response);
    }
}