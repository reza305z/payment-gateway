namespace PaymentApi.DTOs.Payment;

public class VerifyResponse
{
    public string Status { get; set; } = null!;
    public long Amount { get; set; }
    public string ReservationNumber { get; set; } = null!;
    public string? Rrn { get; set; }
}