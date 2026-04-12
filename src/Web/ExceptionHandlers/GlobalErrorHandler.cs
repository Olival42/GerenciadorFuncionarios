namespace GerenciadorFuncionarios.Web.ExceptionHandlers;

using GerenciadorFuncionarios.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Auth.Domain.Exceptions;

public class GlobalErrorHandler : IExceptionFilter
{
    private readonly ILogger<GlobalErrorHandler> _logger;

    public GlobalErrorHandler(ILogger<GlobalErrorHandler> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        var (statusCode, error) = exception switch
        {
            EntityNotFoundException ex => (
                StatusCodes.Status404NotFound,
                new ErrorResponse("ENTITY_NOT_FOUND", ex.Message)
            ),

            EntityAlreadyExistsException ex => (
                StatusCodes.Status409Conflict,
                new ErrorResponse("ENTITY_ALREADY_EXISTS", ex.Message)
            ),

            CPFAlreadyExistsException ex => (
                StatusCodes.Status409Conflict,
                new ErrorResponse("CPF_ALREADY_EXISTS", ex.Message)
            ),

            UserNameAlreadyExistsException ex => (
                StatusCodes.Status409Conflict,
                new ErrorResponse("EMAIL_ALREADY_EXISTS", ex.Message)
            ),

            InactiveEntityException ex => (
                StatusCodes.Status409Conflict,
                new ErrorResponse("ENTITY_INACTIVE", ex.Message)
            ),

            BadRequestException ex => (
                StatusCodes.Status400BadRequest,
                new ErrorResponse("BAD_REQUEST", ex.Message)
            ),

            BadCredentialsException ex => (
                StatusCodes.Status401Unauthorized,
                new ErrorResponse("BAD_CREDENTIALS", ex.Message)
            ),

            UnauthorizedAccessException ex => (
                StatusCodes.Status401Unauthorized,
                new ErrorResponse("ACCESS_UNAUTHORIZED", ex.Message)
            ),

            InvalidOperationException ex => (
                StatusCodes.Status400BadRequest,
                new ErrorResponse("INVALID_OPERATION", ex.Message)
            ),

            ArgumentException ex => (
                StatusCodes.Status400BadRequest,
                new ErrorResponse("ARGUMENT_EXCEPTION", ex.Message)
            ),

            Exception ex => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse("INTERNAL_SERVER_ERROR", ex.Message)
            )
        };

        _logger.LogWarning(
            exception,
            "Erro {Code} em {Method} {Path}",
            error.Code,
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path
        );

        context.Result = new ObjectResult(ApiResponse<ErrorResponse>.Fail(error))
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
