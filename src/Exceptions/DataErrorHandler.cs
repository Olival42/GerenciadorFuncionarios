namespace GerenciadorFuncionarios.Exceptions;

using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

public class DataErrorHandler
{
    public static BadRequestObjectResult OnException(ActionContext context)
    {
        var logger = context.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("Validation");

        var errors = context.ModelState
            .Where(x => x.Value!.Errors.Count() > 0)
            .SelectMany(x => x.Value!.Errors.Select(e =>
                    new DataErrors
                    (
                        Field: x.Key,
                        Message: e.ErrorMessage
                    )
                )
            ).ToList();

        logger.LogWarning(
            "Erro de validação. Path: {Path} Erros: {Count}",
            context.HttpContext.Request.Path,
            errors.Count());

        var response = new ApiResponse<List<DataErrors>>(
            Success: false,
            Data: errors,
            Error: null,
            Timestamp: DateTimeOffset.UtcNow
        );

        return new BadRequestObjectResult(response);
    }
}