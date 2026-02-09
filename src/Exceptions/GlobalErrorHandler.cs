using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GerenciadorFuncionarios.Shared.Responses;
using System;
using GerenciadorFuncionarios.Exceptions;

public class GlobalErrorHandler : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var (statusCode, error) = context.Exception switch
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

            EmailAlreadyExistsException ex => (
                StatusCodes.Status409Conflict,
                new ErrorResponse("EMAIL_ALREADY_EXISTS", ex.Message)
            ),

            InactiveEntityException ex => (
                StatusCodes.Status409Conflict,
                new ErrorResponse("ENTITY_INACTIVE", ex.Message)
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse("INTERNAL_SERVER_ERROR", "Erro interno no servidor")
            )
        };

        context.Result = new ObjectResult(ApiResponse<ErrorResponse>.Fail(error))
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}

