using Microsoft.EntityFrameworkCore;

using PaymentApi.Data;
using PaymentApi.DTOs.Payment;
using PaymentApi.Exceptions;
using PaymentApi.Models;

namespace PaymentApi.Services;

public class PaymentService(
    AppDbContext db,
    IConfiguration configuration,
    IRedirectUrlValidator redirectUrlValidator) : IPaymentService
{
    private readonly AppDbContext _db = db;
    private readonly IConfiguration _configuration = configuration;
    private readonly IRedirectUrlValidator _redirectUrlValidator = redirectUrlValidator;

    public async Task<GetTokenResponse> GetTokenAsync(
        GetTokenRequest request,
        CancellationToken cancellationToken)
    {
        _redirectUrlValidator.Validate(request.RedirectUrl);

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
        await _db.SaveChangesAsync(cancellationToken);

        var frontendUrl = _configuration["FrontendUrl"];
        var gatewayUrl = $"{frontendUrl}/gateway/{token}";

        return new GetTokenResponse
        {
            IsSuccess = true,
            Token = token,
            GatewayUrl = gatewayUrl
        };
    }

    public async Task<VerifyResponse> VerifyAsync(
        VerifyRequest request,
        CancellationToken cancellationToken)
    {
        var transaction = await _db.PaymentTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Token == request.Token,
                cancellationToken);

        if (transaction is null)
        {
            throw new PaymentTransactionNotFoundException(
                "Payment transaction was not found.");
        }

        return new VerifyResponse
        {
            Status = transaction.Status.ToString(),
            Amount = transaction.Amount,
            ReservationNumber = transaction.ReservationNumber,
            Rrn = transaction.Rrn
        };
    }
}
