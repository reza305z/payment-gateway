using Microsoft.EntityFrameworkCore;
using PaymentApi.Data;
using PaymentApi.Services;
using PaymentApi.Configuration;
using PaymentApi.BackgroundServices;
using PaymentApi.Infrastructure.ExceptionHandling;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IRedirectUrlValidator, RedirectUrlValidator>();

builder.Services.AddHostedService<PaymentExpirationService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
    httpContext =>
    {
        var ipAddress =
            httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ipAddress,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });

    options.AddPolicy("payment", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services
    .AddOptions<PaymentOptions>()
    .Bind(builder.Configuration.GetSection(PaymentOptions.SectionName))
    .Validate(
        options => options.AllowedRedirectOrigins.Length > 0,
        "At least one allowed redirect origin must be configured.")
    .Validate(
        options => options.ExpirationTimeoutSeconds > 0,
        "Payment expiration timeout must be greater than zero.")
    .Validate(
        options => options.ExpirationIntervalSeconds > 0,
        "Payment expiration interval must be greater than zero.")
    .ValidateOnStart();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// NSwag serves the Swagger UI used to explore the API in development.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApiDocument(document =>
    {
        document.Title = "Payment Gateway API";
        document.Version = "v1";
        document.AllowNullableBodyParameters = false;
    });
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();
