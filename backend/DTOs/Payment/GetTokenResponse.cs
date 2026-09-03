namespace PaymentApi.DTOs.Payment;

public class GetTokenResponse
{
    public bool IsSuccess { get; set; }

    public Guid Token { get; set; }

    public string GatewayUrl { get; set; } = null!;
}