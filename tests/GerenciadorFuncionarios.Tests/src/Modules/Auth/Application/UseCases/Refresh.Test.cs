using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;
using GerenciadorFuncionarios.Modules.Auth.Application.UseCases;
using GerenciadorFuncionarios.Infrastructure.Cache.Interfaces;

public class RefreshUseCaseTests
{
    private readonly Mock<IFuncionarioRepository> _mockRepository;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IRedisService> _mockRedis;

    private readonly Refresh _useCase;

    public RefreshUseCaseTests()
    {
        _mockRepository = new Mock<IFuncionarioRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _mockRedis = new Mock<IRedisService>();

        _useCase = new Refresh(
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
    public async Task Execute_Should_Return_Token_When_Refresh_Token_Is_Valid()
    {
        var refreshToken = "refresh-token";

        var func = Create_Funcionario();


        _mockRedis
            .Setup(r => r.GetAsync($"refresh:{refreshToken}"))
            .ReturnsAsync(func.Id.ToString());

        _mockRedis
            .Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        _mockRedis.Setup(r => r.DeleteAsync($"refresh:{refreshToken}"))
                   .Returns(Task.CompletedTask);

        _mockRepository
            .Setup(r => r.GetByIdAsync(func.Id))
            .ReturnsAsync(func);

        _mockJwtService
            .Setup(j => j.GenerateAccessToken(It.IsAny<Funcionario>()))
            .Returns(("access-token", new DateTimeOffset(DateTime.UtcNow.AddHours(1)).ToUnixTimeSeconds()));

        _mockJwtService
            .Setup(j => j.GenerateRefreshToken(It.IsAny<Funcionario>()))
            .Returns("refresh-token");

        var response = await _useCase.Execute(refreshToken);

        Assert.NotNull(response);
        Assert.Equal("access-token", response.Data!.AccessToken);
        Assert.Equal("refresh-token", response.Data.RefreshToken);

        _mockRedis.Verify(r =>
            r.DeleteAsync($"refresh:refresh-token"),
            Times.Once);

        _mockRedis.Verify(r => r.SetAsync(
            $"refresh:refresh-token",
            func.Id.ToString(),
            It.IsAny<TimeSpan>()
        ), Times.Once);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_Refresh_Token_Not_Found()
    {
        _mockRedis
            .Setup(r => r.GetAsync("refresh-token"))
            .ReturnsAsync((string?)null);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _useCase.Execute("refresh-token")
        );

        Assert.Equal("Refresh token inválido", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_User_NotFound()
    {
        _mockRedis
           .Setup(r => r.GetAsync("refresh-token"))
           .ReturnsAsync("123");

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Funcionario?)null);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _useCase.Execute("refresh-token")
        );

        Assert.Equal("Refresh token inválido", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_Redis_Set_Fails()
    {
        var refreshToken = "refresh-token";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        var userId = Guid.NewGuid();
        var func = Create_Funcionario();

        _mockRedis.Setup(r => r.GetAsync($"refresh:{refreshToken}"))
            .ReturnsAsync(userId.ToString());

        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(func);

        _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<Funcionario>()))
            .Returns(("access-token", DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
        _mockJwtService.Setup(j => j.GenerateRefreshToken(It.IsAny<Funcionario>()))
            .Returns("new-refresh-token");

        _mockRedis.Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ThrowsAsync(new Exception("Redis error"));

        await Assert.ThrowsAsync<Exception>(() => _useCase.Execute(refreshToken));
    }

        [Fact]
    public async Task Execute_Should_Set_Access_Token_Expiration_Correctly()
    {
        var refreshToken = "refresh-token";
        var func = Create_Funcionario();

        _mockRedis.Setup(r => r.GetAsync($"refresh:{refreshToken}"))
            .ReturnsAsync(func.Id.ToString());

        _mockRepository.Setup(r => r.GetByIdAsync(func.Id))
            .ReturnsAsync(func);

        var expiration = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();

        _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<Funcionario>()))
            .Returns(("access-token", expiration));

        _mockJwtService.Setup(j => j.GenerateRefreshToken(It.IsAny<Funcionario>()))
            .Returns("new-refresh-token");

        var response = await _useCase.Execute(refreshToken);

        Assert.Equal(expiration, response.Data!.ExpiresAt);
    }
}
