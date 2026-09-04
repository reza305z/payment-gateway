using System.ComponentModel.DataAnnotations;

namespace PaymentApi.DTOs.Payment;

public abstract class TokenRequestBase : IValidatableObject
{
    public Guid Token { get; set; }

    public virtual IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (Token == Guid.Empty)
        {
            yield return new ValidationResult(
                "Token is required.",
                [nameof(Token)]);
        }
    }
}