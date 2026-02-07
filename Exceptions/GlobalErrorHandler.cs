using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GerenciadorFuncionarios.Shared.Responses;
using System;
using GerenciadorFuncionarios.Exceptions;

public class GlobalErrorHandler : IExceptionFilter
{
	public void OnException(ExceptionContext context)
	{
		if (context.Exception is EntityNotFoundException)
		{
			var error = new ErrorResponse
				(
					Code: "ENTITY_NOT_FOUND",
					Message: context.Exception.Message
				);

            context.Result = new NotFoundObjectResult(ApiResponse<object>.Fail(error));
            context.ExceptionHandled = true;
        }

		if (context.Exception is EntityAlreadyExistsException)
		{
            var error = new ErrorResponse
                (
                    Code: "ENTITY_ALREADY_EXISTS",
                    Message: context.Exception.Message
                );

            context.Result = new ConflictObjectResult(ApiResponse<object>.Fail(error));
            context.ExceptionHandled = true;
        }
	}
}
