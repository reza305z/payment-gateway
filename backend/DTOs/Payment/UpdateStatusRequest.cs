using System.ComponentModel.DataAnnotations;

namespace PaymentApi.DTOs.Payment;

public class UpdateStatusRequest : TokenRequestBase
{
    public bool IsSuccess { get; set; }

    public string? Rrn { get; set; }

    public override IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        foreach (var result in base.Validate(validationContext))
        {
            yield return result;
        }

        if (IsSuccess && (string.IsNullOrWhiteSpace(Rrn) || Rrn.Length != 12))
        {
            yield return new ValidationResult(
                "A 12-digit RRN is required for a successful payment.",
                [nameof(Rrn)]);
        }

        if (!string.IsNullOrWhiteSpace(Rrn) && !Rrn.All(char.IsDigit))
        {
            yield return new ValidationResult(
                "RRN must contain only digits.",
                [nameof(Rrn)]);
        }
    }
}