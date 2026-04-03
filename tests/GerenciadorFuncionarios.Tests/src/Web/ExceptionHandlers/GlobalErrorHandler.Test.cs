using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using GerenciadorFuncionarios.Web.ExceptionHandlers;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Auth.Domain.Exceptions;

public class GlobalErrorHandlerTests
{

    private readonly Mock<ILogger<GlobalErrorHandler>> _mockLogger;
    private readonly GlobalErrorHandler _handler;

    public GlobalErrorHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GlobalErrorHandler>>();
        _handler = new GlobalErrorHandler(_mockLogger.Object);
    }

    private ExceptionContext CreateContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();

        var actionContext = new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new ActionDescriptor()
        );

        return new ExceptionContext(
            actionContext,
            new List<IFilterMetadata>()
        )
        {
            Exception = exception
        };
    }

    [Fact]
    public void OnException_Should_Return_404_When_EntityNotFoundException()
    {
        var context = CreateContext(new EntityNotFoundException("Not found"));

        _handler.OnException(context);

        var result = Assert.IsType<ObjectResult>(context.Result);

        Assert.Equal(404, result.StatusCode);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_Should_Return_409_When_EntityAlreadyExistsException()
    {
        var context = CreateContext(new EntityAlreadyExistsException("Already exists"));

        _handler.OnException(context);

        var result = Assert.IsType<ObjectResult>(context.Result);

        Assert.Equal(409, result.StatusCode);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_Should_Return_409_When_CPFAlreadyExistsException()
    {
        var context = CreateContext(new CPFAlreadyExistsException("CPF Already exists"));

        _handler.OnException(context);

        var result = Assert.IsType<ObjectResult>(context.Result);

        Assert.Equal(409, result.StatusCode);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_Should_Return_409_When_EmailAlreadyExistsException()
    {
        var context = CreateContext(new UserNameAlreadyExistsException("Email Already exists"));

        _handler.OnException(context);

        var result = Assert.IsType<ObjectResult>(context.Result);

        Assert.Equal(409, result.StatusCode);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_Should_Return_409_When_InactiveEntityException()
    {
        var context = CreateContext(new InactiveEntityException("Inactive entity"));

        _handler.OnException(context);

        var result = Assert.IsType<ObjectResult>(context.Result);

        Assert.Equal(409, result.StatusCode);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_Should_Return_401_When_UnauthorizedAccessException()
    {
        var context = CreateContext(new UnauthorizedAccessException("Unauthorized access"));

        _handler.OnException(context);

        var result = Assert.IsType<ObjectResult>(context.Result);

        Assert.Equal(401, result.StatusCode);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_Should_Return_401_When_BadCredentialsException()
    {
        var context = CreateContext(new BadCredentialsException("Bad credentials"));

        _handler.OnException(context);

        var result = Assert.IsType<ObjectResult>(context.Result);

        Assert.Equal(401, result.StatusCode);
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_Should_Return_500_When_Exception()
    {
        var context = CreateContext(new Exception("Internal server error"));

        _handler.OnException(context);

        var result = Assert.IsType<ObjectResult>(context.Result);

        Assert.Equal(500, result.StatusCode);
        Assert.True(context.ExceptionHandled);
    }
}