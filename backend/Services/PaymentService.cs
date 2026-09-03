using PaymentApi.Data;
using PaymentApi.DTOs.Payment;
using PaymentApi.Models;

namespace PaymentApi.Services;

public class PaymentService(
    AppDbContext db,
    IConfiguration configuration) : IPaymentService
{
    private readonly AppDbContext _db = db;
    private readonly IConfiguration _configuration = configuration;

    public async Task<GetTokenResponse> GetTokenAsync(GetTokenRequest request)
    {
        var token = Guid.NewGuid();
        var transaction = new PaymentTransaction
        {
            Token = token,
            TerminalNo = request.TerminalNo,
            Amount = request.Amount,
            RedirectUrl = request.RedirectUrl,
            ReservationNumber = request.ReservationNumber,
            PhoneNumber = request.PhoneNumber,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _db.PaymentTransactions.Add(transaction);
        await _db.SaveChangesAsync();

        var frontendUrl = _configuration["FrontendUrl"];
        var gatewayUrl = $"{frontendUrl}/gateway/{token}";

        return new GetTokenResponse
        {
            IsSuccess = true,
            Token = token,
            GatewayUrl = gatewayUrl
        };
    }
}