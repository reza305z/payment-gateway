using System.ComponentModel.DataAnnotations;

namespace PaymentApi.DTOs.Payment;

/// <summary>
/// Request body for creating a payment token.
/// </summary>
public class GetTokenRequest
{
    /// <summary>
    /// Terminal number the payment is initiated from.
    /// </summary>
    /// <example>"123456"</example>
    [Required]
    [MaxLength(50)]
    public string TerminalNo { get; set; } = null!;

    /// <summary>
    /// Payment amount in Rials.
    /// </summary>
    /// <example>500000</example>
    [Range(1, long.MaxValue)]
    public long Amount { get; set; }

    /// <summary>
    /// Where the gateway redirects the customer after the payment attempt.
    /// </summary>
    /// <example>http://shop.localhost:3080/payment/result</example>
    [Required]
    [MaxLength(2048)]
    public string RedirectUrl { get; set; } = null!;

    /// <summary>
    /// Merchant-side reservation identifier.
    /// </summary>
    /// <example>RES-123</example>
    [Required]
    [MaxLength(100)]
    public string ReservationNumber { get; set; } = null!;

    /// <summary>
    /// Customer phone number (Iranian mobile format).
    /// </summary>
    /// <example>09121234567</example>
    [Required]
    [RegularExpression(@"^09\d{9}$")]
    public string PhoneNumber { get; set; } = null!;
}