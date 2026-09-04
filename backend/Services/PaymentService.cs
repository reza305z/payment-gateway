using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using PaymentApi.Configuration;
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

    public async Task UpdateStatusAsync(
        UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Token == Guid.Empty)
        {
            throw new InvalidPaymentRequestException(
                "Token is invalid.");
        }

        var transactionExists = await _db.PaymentTransactions
            .AsNoTracking()
            .AnyAsync(x => x.Token == request.Token, cancellationToken);

        if (!transactionExists)
        {
            throw new PaymentTransactionNotFoundException(
                "Payment transaction was not found.");
        }

        var newStatus = request.IsSuccess
            ? PaymentStatus.Success
            : PaymentStatus.Failed;

        var updatedRows = await _db.PaymentTransactions
            .Where(x =>
                x.Token == request.Token &&
                x.Status == PaymentStatus.Pending)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.Status, newStatus)
                    .SetProperty(
                        x => x.Rrn,
                        request.IsSuccess ? request.Rrn : null)
                    .SetProperty(
                        x => x.UpdatedAt,
                        DateTime.UtcNow),
                cancellationToken);

        if (updatedRows == 0)
        {
            throw new InvalidPaymentRequestException(
                "Payment transaction has already been finalized.");
        }
    }
}
