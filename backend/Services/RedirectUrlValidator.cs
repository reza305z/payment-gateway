using Microsoft.Extensions.Options;
using PaymentApi.Configuration;
using PaymentApi.Exceptions;

namespace PaymentApi.Services;

public interface IRedirectUrlValidator
{
    void Validate(string redirectUrl);
}

public class RedirectUrlValidator(IOptions<PaymentOptions> options) : IRedirectUrlValidator
{
    private readonly PaymentOptions _options = options.Value;

    public void Validate(string redirectUrl)
    {
        if (!Uri.TryCreate(
                redirectUrl,
                UriKind.Absolute,
                out var requestedUri))
        {
            throw new InvalidPaymentRequestException(
                "Redirect URL is invalid.");
        }

        if (!string.Equals(
                requestedUri.Scheme,
                Uri.UriSchemeHttps,
                StringComparison.OrdinalIgnoreCase)
            &&
            !string.Equals(
                requestedUri.Scheme,
                Uri.UriSchemeHttp,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidPaymentRequestException(
                "Redirect URL must use HTTP or HTTPS.");
        }

        var isAllowed = _options.AllowedRedirectOrigins
            .Select(origin =>
            {
                return Uri.TryCreate(
                    origin,
                    UriKind.Absolute,
                    out var allowedUri)
                    ? allowedUri
                    : null;
            })
            .Where(uri => uri is not null)
            .Any(allowedUri =>
                string.Equals(
                    requestedUri.Scheme,
                    allowedUri!.Scheme,
                    StringComparison.OrdinalIgnoreCase)
                &&
                string.Equals(
                    requestedUri.Host,
                    allowedUri.Host,
                    StringComparison.OrdinalIgnoreCase)
                &&
                requestedUri.Port == allowedUri.Port);

        if (!isAllowed)
        {
            throw new InvalidPaymentRequestException(
                "Redirect URL is not an allowed destination.");
        }
    }
}