namespace GerenciadorFuncionarios.Infrastructure.Security;

using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public static class JwtEventsConfig
{
    public static JwtBearerEvents GetEvents()
    {
        return new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.HttpContext.Response.StatusCode = 401;
                await context.HttpContext.Response.WriteAsJsonAsync(ApiResponse<ErrorResponse>
                    .Fail(new ErrorResponse("UNAUTHORIZED", "Invalid or missing token.")));
            },

            OnForbidden = async context =>
            {
                context.HttpContext.Response.StatusCode = 403;
                await context.HttpContext.Response.WriteAsJsonAsync(ApiResponse<ErrorResponse>
                    .Fail(new ErrorResponse("FORBIDDEN", "Access denied.")));
            }
        };
    }
}