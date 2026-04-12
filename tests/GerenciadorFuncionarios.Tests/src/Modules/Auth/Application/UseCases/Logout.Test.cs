using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;
using GerenciadorFuncionarios.Modules.Auth.Application.UseCases;
using GerenciadorFuncionarios.Infrastructure.Cache.Interfaces;

public class LogoutUseCaseTests
{
    private readonly Mock<IRedisService> _mockRedis;
    private readonly Mock<IUserContextService> _mockUserContext;

    private readonly Logout _useCase;

    public LogoutUseCaseTests()
    {
        _mockRedis = new Mock<IRedisService>();
        _mockUserContext = new Mock<IUserContextService>();

        _useCase = new Logout(
            _mockRedis.Object,
            _mockUserContext.Object
        );
    }

    [Fact]
    public async Task Execute_Should_Delete_Refresh_Token_When_Valid()
    {
        var refresh = "refresh-token";
        var id = Guid.NewGuid();

        _mockRedis
            .Setup(r => r.GetAsync($"refresh:{refresh}"))
            .ReturnsAsync(id.ToString());

        _mockUserContext
            .Setup(r => r.GetUserId())
            .Returns(id.ToString());

        _mockRedis.Setup(r => r.DeleteAsync($"refresh:{refresh}"))
                   .Returns(Task.CompletedTask);

        await _useCase.Execute("refresh-token");

        _mockRedis.Verify(r =>
            r.DeleteAsync($"refresh:refresh-token"),
            Times.Once);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_Refresh_Token_Not_Found()
    {
        _mockRedis
            .Setup(r => r.GetAsync("refresh-token"))
            .ReturnsAsync((string?)null);

        _mockUserContext
            .Setup(r => r.GetUserId())
            .Returns("123");

        _mockRedis.Setup(r => r.DeleteAsync("refresh-token"))
                   .Returns(Task.CompletedTask);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _useCase.Execute("refresh-token")
        );

        Assert.Equal("Refresh token inválido", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_Refresh_Token_Does_Not_Belong_To_User()
    {
        _mockRedis
            .Setup(r => r.GetAsync("refresh-token"))
            .ReturnsAsync("123");

        _mockUserContext
            .Setup(r => r.GetUserId())
            .Returns("456");

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _useCase.Execute("refresh-token")
        );

        Assert.Equal("Refresh token inválido", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Execute_Should_Throw_Exception_When_Token_Is_Null_Or_Empty(string invalidToken)
    {
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _useCase.Execute(invalidToken)
        );

        Assert.Equal("Refresh token inválido", exception.Message);
    }

    [Fact]
    public async Task Execute_Should_Throw_Exception_When_Redis_Fails()
    {
        var refreshToken = "refresh-token";

        _mockRedis
            .Setup(r => r.GetAsync($"refresh:{refreshToken}"))
            .ThrowsAsync(new Exception("Redis falhou"));

        _mockUserContext
            .Setup(r => r.GetUserId())
            .Returns("123");

        var ex = await Assert.ThrowsAsync<Exception>(() => _useCase.Execute(refreshToken));

        Assert.Equal("Redis falhou", ex.Message);

        _mockRedis.Verify(r => r.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Execute_Should_Throw_Exception_When_Redis_Delete_Fails()
    {
        var refreshToken = "refresh-token";

        _mockRedis
            .Setup(r => r.GetAsync($"refresh:{refreshToken}"))
            .ReturnsAsync("123");

        _mockUserContext
            .Setup(r => r.GetUserId())
            .Returns("123");

        _mockRedis
            .Setup(r => r.DeleteAsync($"refresh:{refreshToken}"))
            .ThrowsAsync(new Exception("Falha ao deletar no Redis"));

        var ex = await Assert.ThrowsAsync<Exception>(() => _useCase.Execute(refreshToken));

        Assert.Equal("Falha ao deletar no Redis", ex.Message);

        _mockRedis.Verify(r => r.GetAsync($"refresh:{refreshToken}"), Times.Once);

        _mockRedis.Verify(r => r.DeleteAsync($"refresh:{refreshToken}"), Times.Once);
    }
}
