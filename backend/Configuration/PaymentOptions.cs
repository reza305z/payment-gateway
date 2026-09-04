namespace PaymentApi.Configuration;

public class PaymentOptions
{
    public const string SectionName = "Payment";
    public string[] AllowedRedirectOrigins { get; set; } = [];
}