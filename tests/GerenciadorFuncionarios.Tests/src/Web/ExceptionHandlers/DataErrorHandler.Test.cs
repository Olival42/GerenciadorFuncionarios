using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.Extensions.DependencyInjection;
using GerenciadorFuncionarios.Web.ExceptionHandlers;

public class DataErrorHandlerTests
{

    private ActionContext CreateContext()
    {
        var httpContext = new DefaultHttpContext();

        var services = new ServiceCollection();
        services.AddLogging();

        httpContext.RequestServices = services.BuildServiceProvider();

        return new ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new ActionDescriptor(),
            new ModelStateDictionary()
        );
    }

    [Fact]
    public void OnException_Should_Return_BadRequest_With_Errors()
    {
        var actionContext = CreateContext();

        actionContext.ModelState.AddModelError("Name", "Name is required");

        var result = DataErrorHandler.OnException(actionContext);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);

        var response = Assert.IsType<ApiResponse<List<DataErrors>>>(badRequest.Value);

        Assert.False(response.Success);
        Assert.Single(response.Data!);
        Assert.Equal("Name", response.Data![0].Field);
        Assert.Equal("Name is required", response.Data[0].Message);
    }

    [Fact]
    public void OnException_Should_Return_Multiple_Errors()
    {
        var actionContext = CreateContext();

        actionContext.ModelState.AddModelError("Name", "Name is required");
        actionContext.ModelState.AddModelError("Cpf", "CPF is required");

        var result = DataErrorHandler.OnException(actionContext);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);

        var response = Assert.IsType<ApiResponse<List<DataErrors>>>(badRequest.Value);

        Assert.Equal(2, response.Data!.Count());
    }

    [Fact]
    public void OnException_Should_Handle_Multiple_Errors_Same_Field()
    {
        var actionContext = CreateContext();

        actionContext.ModelState.AddModelError("Email", "Required");
        actionContext.ModelState.AddModelError("Email", "Invalid format");

        var result = DataErrorHandler.OnException(actionContext);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);

        var response = Assert.IsType<ApiResponse<List<DataErrors>>>(badRequest.Value);

        Assert.Equal(2, response.Data!.Count);
    }
}