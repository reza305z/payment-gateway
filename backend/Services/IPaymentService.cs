using PaymentApi.DTOs.Payment;

namespace PaymentApi.Services;

public interface IPaymentService
{
    Task<GetTokenResponse> GetTokenAsync(
        GetTokenRequest request, CancellationToken cancellationToken);

    Task<VerifyResponse> VerifyAsync(
        VerifyRequest request, CancellationToken cancellationToken);

    Task UpdateStatusAsync(
        UpdateStatusRequest request, CancellationToken cancellationToken);

    Task<TransactionResponse> GetTransactionAsync(
        Guid token, CancellationToken cancellationToken);
}
