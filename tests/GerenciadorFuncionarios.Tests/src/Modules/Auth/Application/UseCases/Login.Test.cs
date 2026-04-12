using Xunit;
using Moq;
using BCrypt.Net;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Infrastructure.Cache.Interfaces;
using GerenciadorFuncionarios.Modules.Auth.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;
using GerenciadorFuncionarios.Modules.Auth.Application.UseCases;
using GerenciadorFuncionarios.Modules.Auth.Web.Requests;

public class LoginUseCaseTests
{
    private readonly Mock<IFuncionarioRepository> _mockRepository;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IRedisService> _mockRedis;

    private readonly Login _useCase;

    public LoginUseCaseTests()
    {
        _mockRepository = new Mock<IFuncionarioRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _mockRedis = new Mock<IRedisService>();

        _useCase = new Login(
            _mockRepository.Object,
            _mockJwtService.Object,
            _mockRedis.Object
        );
    }

    private Funcionario Create_Funcionario()
    {
        return new Funcionario
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Phone = "44999999999",
            CPF = "68714247097",
            UserName = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("StrongP@ss1"),
            Role = Role.GERENTE,
            IsActive = true
        };
    }

    [Fact]
    public async Task Execute_Should_Return_Token_When_Credentials_Are_Valid()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByUserName("admin123"))
            .ReturnsAsync(func);

        var loginDto = new LoginDTO { UserName = "admin123", Password = "123456" };

        _mockJwtService
            .Setup(j => j.GenerateAccessToken(It.IsAny<Funcionario>()))
            .Returns(("access-token", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()));

        _mockJwtService
            .Setup(j => j.GenerateRefreshToken(It.IsAny<Funcionario>()))
            .Returns("refresh-token");

        _mockRedis
            .Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        var response = await _useCase.Execute(loginDto);

        Assert.NotNull(response);
        Assert.Equal("access-token", response.Data!.AccessToken);
        Assert.Equal("refresh-token", response.Data.RefreshToken);
    }

    [Fact]
    public async Task Execute_Should_Return_Error_When_UserName_Is_Invalid()
    {
        _mockRepository
            .Setup(r => r.GetByUserName("admin123"))
            .ReturnsAsync((Funcionario?)null);

        var exception = await Assert.ThrowsAsync<BadCredentialsException>(
            () => _useCase.Execute(new LoginDTO { UserName = "admin123", Password = "123456" })
        );

        Assert.Equal("Credenciais inválidas.", exception.Message);
    }

    [Fact]
    public async Task Execute_Should_Return_Error_When_Password_Is_Invalid()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByUserName("admin123"))
            .ReturnsAsync(func);

        var exception = await Assert.ThrowsAsync<BadCredentialsException>(
            () => _useCase.Execute(new LoginDTO { UserName = "admin123", Password = "12345" })
        );

        Assert.Equal("Credenciais inválidas.", exception.Message);
    }

    [Fact]
    public async Task Execute_Should_Return_Error_When_User_Is_Inactive()
    {
        var func = Create_Funcionario();
        func.IsActive = false;

        _mockRepository
            .Setup(r => r.GetByUserName("admin123"))
            .ReturnsAsync(func);

        var exception = await Assert.ThrowsAsync<BadCredentialsException>(
            () => _useCase.Execute(new LoginDTO { UserName = "admin123", Password = "123456" })
        );

        Assert.Equal("Credenciais inválidas.", exception.Message);
    }

    [Fact]
    public async Task Execute_Should_Throw_Error_When_UserName_Is_NullOrEmpty()
    {
        var loginDto = new LoginDTO { UserName = "", Password = "123456" };

        var ex = await Assert.ThrowsAsync<BadCredentialsException>(
            () => _useCase.Execute(loginDto)
        );

        Assert.Equal("Credenciais inválidas.", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_Throw_Error_When_Password_Is_NullOrEmpty()
    {
        var loginDto = new LoginDTO { UserName = "admin123", Password = "" };

        var ex = await Assert.ThrowsAsync<BadCredentialsException>(
            () => _useCase.Execute(loginDto)
        );

        Assert.Equal("Credenciais inválidas.", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_Handle_Multiple_Calls_For_Same_User()
    {
        var func = Create_Funcionario();

        _mockRepository.Setup(r => r.GetByUserName("admin123"))
                       .ReturnsAsync(func);

        _mockJwtService.SetupSequence(j => j.GenerateAccessToken(func))
                       .Returns(("access-token-1", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()))
                       .Returns(("access-token-2", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()));

        _mockJwtService.SetupSequence(j => j.GenerateRefreshToken(func))
                       .Returns("refresh-token-1")
                       .Returns("refresh-token-2");

        _mockRedis.Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                         .Returns(Task.CompletedTask);

        var loginDto = new LoginDTO { UserName = "admin123", Password = "123456" };

        var response1 = await _useCase.Execute(loginDto);
        var response2 = await _useCase.Execute(loginDto);

        Assert.NotEqual(response1.Data!.AccessToken, response2.Data!.AccessToken);
        Assert.NotEqual(response1.Data.RefreshToken, response2.Data.RefreshToken);
    }
}
