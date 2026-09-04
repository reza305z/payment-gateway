namespace PaymentApi.Models;

public class PaymentTransaction
{
    public long Id { get; set; }
    public Guid Token { get; set; }
    public string TerminalNo { get; set; } = null!;
    public long Amount { get; set; }
    public string RedirectUrl { get; set; } = null!;
    public string ReservationNumber { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public PaymentStatus Status { get; set; }
    public string? Rrn { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum PaymentStatus
{
    Pending = 0,
    Success = 1,
    Failed = 2,
    Expired = 3
}