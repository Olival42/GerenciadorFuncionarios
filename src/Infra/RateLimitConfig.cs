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
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            }));
    }

    private static void AddAuthLimit(this RateLimiterOptions options)
    {
        options.AddFixedWindowLimiter("Auth", config =>
        {
            config.PermitLimit = 10;
            config.Window = TimeSpan.FromMinutes(1);
            config.QueueLimit = 0;
        });
    }
}