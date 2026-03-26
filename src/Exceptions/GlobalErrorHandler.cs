﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GerenciadorFuncionarios.Shared.Responses;
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

            BadCredentialsException ex => (
                StatusCodes.Status401Unauthorized,
                new ErrorResponse("BAD_CREDENTIALS", ex.Message)
            ),

            UnauthorizedAccessException ex => (
                StatusCodes.Status401Unauthorized,
                new ErrorResponse("ACCESS_UNAUTHORIZED", ex.Message)
            ),

            Exception ex => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse("INTERNAL_SERVER_ERROR", ex.Message)
            )
        };

        context.Result = new ObjectResult(ApiResponse<ErrorResponse>.Fail(error))
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
