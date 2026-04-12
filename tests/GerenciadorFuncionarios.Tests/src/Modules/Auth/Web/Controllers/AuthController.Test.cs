using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.AspNetCore.Http.Features;
using GerenciadorFuncionarios.Modules.Auth.Web.Controllers;
using GerenciadorFuncionarios.Modules.Auth.Web.Requests;
using GerenciadorFuncionarios.Modules.Auth.Web.Responses;
using GerenciadorFuncionarios.Modules.Auth.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Auth.Application.UseCases;

public class AuthControllerTests
{
    private readonly Mock<ILogin> _useCaseLogin;
    private readonly Mock<ILogout> _useCaseLogout;
    private readonly Mock<IRefresh> _useCaseRefresh;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _useCaseLogin = new Mock<ILogin>();
        _useCaseLogout = new Mock<ILogout>();
        _useCaseRefresh = new Mock<IRefresh>();
        _controller = new AuthController(_useCaseLogin.Object, _useCaseLogout.Object, _useCaseRefresh.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task Login_Should_Return_Ok_With_Valid_Data()
    {
        var loginDto = new LoginDTO { UserName = "teste", Password = "1234" };

        var tokenResponse = new TokenResponseDTO(
            AccessToken: "access_token",
            ExpiresAt: DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(),
            RefreshToken: "refresh_token",
            UserName: "teste",
            Role: GerenciadorFuncionarios.Modules.Auth.Domain.Enums.Role.GERENTE
        );

        _useCaseLogin.Setup(s => s.Execute(It.IsAny<LoginDTO>()))
            .ReturnsAsync(ApiResponse<TokenResponseDTO>.Ok(tokenResponse));

        var result = await _controller.Login(loginDto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var apiResponse = Assert.IsType<ApiResponse<AuthResponseDTO>>(okResult.Value);

        Assert.Equal("access_token", apiResponse.Data!.AccessToken);
        Assert.Equal("teste", apiResponse.Data.UserName);
    }

    [Fact]
    public async Task Login_Should_Set_RefreshToken_Cookie()
    {
        var loginDto = new LoginDTO { UserName = "teste", Password = "1234" };

        _useCaseLogin.Setup(s => s.Execute(It.IsAny<LoginDTO>()))
            .ReturnsAsync(ApiResponse<TokenResponseDTO>.Ok(
                new TokenResponseDTO(
                    AccessToken: "access_token",
                    ExpiresAt: DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(),
                    RefreshToken: "refresh_token",
                    UserName: "teste",
                    Role: GerenciadorFuncionarios.Modules.Auth.Domain.Enums.Role.GERENTE
                )
            ));

        await _controller.Login(loginDto);

        var setCookieHeader = _controller.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains("refreshToken=refresh_token", setCookieHeader);
    }

    [Fact]
    public async Task Login_Should_Propagate_Service_Errors()
    {
        var loginDto = new LoginDTO { UserName = "teste", Password = "1234" };

        _useCaseLogin.Setup(s => s.Execute(It.IsAny<LoginDTO>()))
            .ThrowsAsync(new BadCredentialsException("Credenciais inválidas"));

        var ex = await Assert.ThrowsAsync<BadCredentialsException>(
            () => _controller.Login(loginDto));

        Assert.Equal("Credenciais inválidas", ex.Message);
    }

    [Fact]
    public async Task Logout_Should_Return_NoContent_When_Successful()
    {
        var result = await _controller.Logout();

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Logout_Should_Delete_RefreshToken_Cookie()
    {
        await _controller.Logout();

        var setCookieHeader = _controller.Response.Headers["Set-Cookie"].ToString();
        Assert.DoesNotContain("refreshToken=refresh_token", setCookieHeader);
    }

    [Fact]
    public async Task Refresh_Should_Return_Ok_With_Valid_RefreshToken()
    {
        var refreshToken = "refresh_token";

        var tokenResponse = new TokenResponseDTO(
            AccessToken: "access_token",
            ExpiresAt: DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(),
            RefreshToken: "refresh_token",
            UserName: "teste",
            Role: GerenciadorFuncionarios.Modules.Auth.Domain.Enums.Role.GERENTE
        );

        _useCaseRefresh.Setup(s => s.Execute(refreshToken))
            .ReturnsAsync(ApiResponse<TokenResponseDTO>.Ok(tokenResponse));

        var cookiesMock = new Mock<IRequestCookieCollection>();
        cookiesMock.Setup(c => c["refreshToken"]).Returns(refreshToken);

        var cookieFeatureMock = new Mock<IRequestCookiesFeature>();
        cookieFeatureMock.Setup(f => f.Cookies).Returns(cookiesMock.Object);

        _controller.HttpContext.Features.Set<IRequestCookiesFeature>(cookieFeatureMock.Object);

        var result = await _controller.Refresh();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<AuthResponseDTO>>(okResult.Value);

        Assert.Equal("access_token", apiResponse.Data!.AccessToken);
        Assert.Equal("teste", apiResponse.Data.UserName);
    }

    [Fact]
    public async Task Refresh_Should_Set_New_RefreshToken_Cookie()
    {
        var refreshToken = "refresh_token";

        var cookiesMock = new Mock<IRequestCookieCollection>();
        cookiesMock.Setup(c => c["refreshToken"]).Returns(refreshToken);

        var cookieFeatureMock = new Mock<IRequestCookiesFeature>();
        cookieFeatureMock.Setup(f => f.Cookies).Returns(cookiesMock.Object);

        _controller.HttpContext.Features.Set<IRequestCookiesFeature>(cookieFeatureMock.Object);

        var tokenResponse = new TokenResponseDTO(
            AccessToken: "access_token",
            ExpiresAt: DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(),
            RefreshToken: refreshToken,
            UserName: "teste",
            Role: GerenciadorFuncionarios.Modules.Auth.Domain.Enums.Role.GERENTE
        );

        _useCaseRefresh.Setup(s => s.Execute(refreshToken))
            .ReturnsAsync(ApiResponse<TokenResponseDTO>.Ok(tokenResponse));

        await _controller.Refresh();

        var setCookieHeader = _controller.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains("refreshToken=refresh_token", setCookieHeader);
    }

    [Fact]
    public async Task Refresh_Should_Throw_Unauthorized_When_RefreshToken_Missing()
    {
        var context = _controller.ControllerContext.HttpContext;

        var cookiesMock = new Mock<IRequestCookieCollection>();
        cookiesMock.Setup(c => c["refreshToken"]).Returns<string?>(null!);

        var cookieFeatureMock = new Mock<IRequestCookiesFeature>();
        cookieFeatureMock.Setup(f => f.Cookies).Returns(cookiesMock.Object);

        context.Features.Set<IRequestCookiesFeature>(cookieFeatureMock.Object);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _controller.Refresh());

        Assert.Equal("Refresh token é obrigatório", ex.Message);
    }
}