using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentApi.Configuration;
using PaymentApi.Data;
using PaymentApi.Models;

namespace PaymentApi.BackgroundServices;

public class PaymentExpirationService(
    IServiceScopeFactory scopeFactory,
    IOptions<PaymentOptions> options,
    ILogger<PaymentExpirationService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly PaymentOptions _options = options.Value;
    private readonly ILogger<PaymentExpirationService> _logger = logger;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Payment expiration service started.");

        var interval = TimeSpan.FromSeconds(
            _options.ExpirationIntervalSeconds);

        using var timer = new PeriodicTimer(interval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ExpirePendingPaymentsAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Normal application shutdown.
        }
        finally
        {
            _logger.LogInformation(
                "Payment expiration service stopped.");
        }
    }

    private async Task ExpirePendingPaymentsAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var expirationThreshold = DateTime.UtcNow.Subtract(
                TimeSpan.FromSeconds(_options.ExpirationTimeoutSeconds));

            var expiredCount = await db.PaymentTransactions
                .Where(x =>
                    x.Status == PaymentStatus.Pending &&
                    x.CreatedAt < expirationThreshold)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            x => x.Status,
                            PaymentStatus.Expired)
                        .SetProperty(
                            x => x.UpdatedAt,
                            DateTime.UtcNow),
                    cancellationToken);

            if (expiredCount > 0)
            {
                _logger.LogInformation(
                    "Expired {Count} pending payment transactions.",
                    expiredCount);
            }
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            // Normal application shutdown.
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Error while expiring pending payment transactions.");
        }
    }
}