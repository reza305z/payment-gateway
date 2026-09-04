using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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
    [EnableRateLimiting("payment")]
    public async Task<ActionResult<GetTokenResponse>> GetToken(
        GetTokenRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _paymentService.GetTokenAsync(
            request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("verify")]
    public async Task<ActionResult<VerifyResponse>> Verify(
        VerifyRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _paymentService.VerifyAsync(
            request, cancellationToken);
        return Ok(response);
    }
}
