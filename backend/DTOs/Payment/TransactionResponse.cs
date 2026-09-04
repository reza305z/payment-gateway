namespace PaymentApi.DTOs.Payment;

public class TransactionResponse
{
    public Guid Token { get; set; }
    public long Amount { get; set; }
    public string Status { get; set; } = null!;
    public string ReservationNumber { get; set; } = null!;
    public string RedirectUrl { get; set; } = null!;
    public string? Rrn { get; set; }
}
