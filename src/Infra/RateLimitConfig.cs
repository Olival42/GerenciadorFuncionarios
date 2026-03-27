namespace GerenciadorFuncionarios.Infra;

using System.Threading.RateLimiting;
using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.AspNetCore.RateLimiting;

public static class RateLimitConfig
{
    public static void RegisterRateLimits(this RateLimiterOptions options)
    {
        AddGlobalLimit(options);
        AddAuthLimit(options);

        options.OnRejected = async (context, info) =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("RateLimiter");

            var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();

           logger.LogWarning(
                "Rate limit excedido. IP: {IP} Endpoint: {Endpoint}",
                ip,
                context.HttpContext.Request.Path
            );

            context.HttpContext.Response.StatusCode = 429;

            await context.HttpContext.Response.WriteAsJsonAsync(ApiResponse<ErrorResponse>
                .Fail(new ErrorResponse("TOO_MANY_REQUESTS", "Rate limit exceeded.")));
        };
    }

    private static void AddGlobalLimit(this RateLimiterOptions options)
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));
    }

    private static void AddAuthLimit(this RateLimiterOptions options)
    {
        options.AddPolicy("Auth", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));
    }
}