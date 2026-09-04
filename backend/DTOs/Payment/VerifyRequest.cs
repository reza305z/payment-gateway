using System.ComponentModel.DataAnnotations;

namespace PaymentApi.DTOs.Payment;

public class VerifyRequest : TokenRequestBase
{
    [Required]
    [MaxLength(50)]
    public string AppCode { get; set; } = null!;
}