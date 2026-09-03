
using System.ComponentModel.DataAnnotations;

namespace PaymentApi.DTOs.Payment;

public class GetTokenRequest
{
    [Required]
    [MaxLength(50)]
    public string TerminalNo { get; set; } = null!;

    [Required]
    [Range(1, long.MaxValue)]
    public long Amount { get; set; }

    [Required]
    [Url]
    [MaxLength(2048)]
    public string RedirectUrl { get; set; } = null!;


    [Required]
    [MaxLength(100)]
    public string ReservationNumber { get; set; } = null!;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = null!;
}