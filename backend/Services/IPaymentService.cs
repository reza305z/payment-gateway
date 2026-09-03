using PaymentApi.DTOs.Payment;

namespace PaymentApi.Services;

public interface IPaymentService
{
    Task<GetTokenResponse> GetTokenAsync(GetTokenRequest request);
}