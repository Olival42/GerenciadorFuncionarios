using Xunit;
using Moq;
using GerenciadorFuncionarios.Services;
using Microsoft.Extensions.Logging;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.DTOs.Auth.Requests;
using GerenciadorFuncionarios.Adapters;
using GerenciadorFuncionarios.Exceptions;

public class AuthServiceTests
{
    private readonly Mock<IRedisService> _mockRedisService;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IUserContextService> _mockUserContextService;
    private readonly Mock<IFuncionarioRepository> _mockRepository;
    private readonly Mock<ILogger<AuthService>> _mockLogger;

    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockRedisService = new Mock<IRedisService>();
        _mockJwtService = new Mock<IJwtService>();
        _mockUserContextService = new Mock<IUserContextService>();
        _mockLogger = new Mock<ILogger<AuthService>>();
        _mockRepository = new Mock<IFuncionarioRepository>();

        _authService = new AuthService(
            _mockRepository.Object,
            _mockJwtService.Object,
            _mockRedisService.Object,
            _mockUserContextService.Object,
            _mockLogger!.Object
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
            Email = "teste@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = GerenciadorFuncionarios.Enums.Role.ADMIN,
            IsActive = true
        };
    }


    [Fact]
    public async Task Login_Should_Return_Token_When_Credentials_Are_Valid()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByEmail(It.Is<string>(email => email == "teste@email.com")))
            .ReturnsAsync(func);

        var loginDto = new LoginDTO{Email = "teste@email.com", Password = "123456"};

        _mockJwtService
            .Setup(j => j.GenerateAccessToken(It.IsAny<Funcionario>()))
            .Returns(("access-token", new DateTimeOffset(DateTime.UtcNow.AddHours(1)).ToUnixTimeSeconds()));

        _mockJwtService
            .Setup(j => j.GenerateRefreshToken(It.IsAny<Funcionario>()))
            .Returns("refresh-token");

        _mockRedisService
            .Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        var response = await _authService.Login(loginDto);

        Assert.NotNull(response);
        Assert.Equal("access-token", response.Data!.AccessToken);
        Assert.Equal("refresh-token", response.Data.RefreshToken);

        _mockRedisService.Verify(r => r.SetAsync(
            $"refresh:refresh-token",
            func.Id.ToString(),
            It.IsAny<TimeSpan>()
        ), Times.Once);
    }

    [Fact]
    public async Task Login_Should_Return_Error_When_Email_Is_Invalid()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByEmail(It.Is<string>(email => email == "test@email.com")))
            .ReturnsAsync((Funcionario?)null);

        var exception = await Assert.ThrowsAsync<BadCredentialsException>(
            () => _authService.Login(new LoginDTO{Email = "teste@email.com", Password = "123456"})
        );

        Assert.Equal("Credenciais inválidas.", exception.Message);
    }

    [Fact]
    public async Task Login_Should_Return_Error_When_Password_Is_Invalid()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByEmail(It.Is<string>(email => email == "teste@email.com")))
            .ReturnsAsync(func);

        var exception = await Assert.ThrowsAsync<BadCredentialsException>(
            () => _authService.Login(new LoginDTO{Email = "teste@email.com", Password = "12345"})
        );

        Assert.Equal("Credenciais inválidas.", exception.Message);
    }

    [Fact]
    public async Task Login_Should_Return_Error_When_User_Is_Inactive()
    {
        var func = Create_Funcionario();
        func.IsActive = false;

        _mockRepository
            .Setup(r => r.GetByEmail(It.Is<string>(email => email == "teste@email.com")))
            .ReturnsAsync(func);

        var exception = await Assert.ThrowsAsync<BadCredentialsException>(
            () => _authService.Login(new LoginDTO{Email = "test@email.com", Password = "123456"})
        );

        Assert.Equal("Credenciais inválidas.", exception.Message);
    }

    [Fact]
    public async Task Login_Should_Throw_Exception_When_Redis_Fails()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByEmail(It.Is<string>(email => email == "teste@email.com")))
            .ReturnsAsync(func);

        _mockJwtService
            .Setup(j => j.GenerateAccessToken(It.IsAny<Funcionario>()))
            .Returns(("access-token", new DateTimeOffset(DateTime.UtcNow.AddHours(1)).ToUnixTimeSeconds()));

        _mockJwtService
            .Setup(j => j.GenerateRefreshToken(It.IsAny<Funcionario>()))
            .Returns("refresh-token");

        _mockRedisService
            .Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ThrowsAsync(new Exception("Redis falhou"));

        var loginDto = new LoginDTO{Email = "teste@email.com", Password = "123456"};

        var ex = await Assert.ThrowsAsync<Exception>(
            () => _authService.Login(loginDto)
        );

        Assert.Equal("Redis falhou", ex.Message);
    }

    [Fact]
    public async Task Login_Should_Throw_Exception_When_Generate_Access_Token_Fails()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByEmail(It.Is<string>(email => email == "teste@email.com")))
            .ReturnsAsync(func);

        _mockJwtService
            .Setup(j => j.GenerateAccessToken(It.IsAny<Funcionario>()))
            .Throws(new Exception("Jwt falhou"));

        _mockJwtService
            .Setup(j => j.GenerateRefreshToken(It.IsAny<Funcionario>()))
            .Returns("refresh-token");

        _mockRedisService
            .Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

        var loginDto = new LoginDTO{Email = "teste@email.com", Password = "123456"};

        var ex = await Assert.ThrowsAsync<Exception>(
            () => _authService.Login(loginDto)
        );

        Assert.Equal("Jwt falhou", ex.Message);
    }

    [Fact]
    public async Task Login_Should_Throw_Exception_When_Generate_Refresh_Token_Fails()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByEmail(It.Is<string>(email => email == "teste@email.com")))
            .ReturnsAsync(func);

        _mockJwtService
            .Setup(j => j.GenerateAccessToken(It.IsAny<Funcionario>()))
            .Returns(("access-token", new DateTimeOffset(DateTime.UtcNow.AddHours(1)).ToUnixTimeSeconds()));

        _mockJwtService
            .Setup(j => j.GenerateRefreshToken(It.IsAny<Funcionario>()))
            .Throws(new Exception("Jwt falhou"));

        _mockRedisService
            .Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

        var loginDto = new LoginDTO{Email = "teste@email.com", Password = "123456"};;

        var ex = await Assert.ThrowsAsync<Exception>(
            () => _authService.Login(loginDto)
        );

        Assert.Equal("Jwt falhou", ex.Message);
    }

    [Fact]
    public async Task Login_Should_Throw_Error_When_Email_Is_NullOrEmpty()
    {
        var loginDto = new LoginDTO{Email = "", Password = "123456"};

        var ex = await Assert.ThrowsAsync<BadCredentialsException>(
            () => _authService.Login(loginDto)
        );

        Assert.Equal("Credenciais inválidas.", ex.Message);
    }

    [Fact]
    public async Task Login_Should_Throw_Error_When_Password_Is_NullOrEmpty()
    {
        var loginDto = new LoginDTO{Email = "teste@email.com", Password = ""};

        var ex = await Assert.ThrowsAsync<BadCredentialsException>(
            () => _authService.Login(loginDto)
        );

        Assert.Equal("Credenciais inválidas.", ex.Message);
    }

    [Fact]
    public async Task Login_Should_Handle_Multiple_Calls_For_Same_User()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        var func = Create_Funcionario();

        _mockRepository.Setup(r => r.GetByEmail("teste@email.com"))
                       .ReturnsAsync(func);

        _mockJwtService.SetupSequence(j => j.GenerateAccessToken(func))
                       .Returns(("access-token-1", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()))
                       .Returns(("access-token-2", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()));

        _mockJwtService.SetupSequence(j => j.GenerateRefreshToken(func))
                       .Returns("refresh-token-1")
                       .Returns("refresh-token-2");

        _mockRedisService.Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                         .Returns(Task.CompletedTask);

        var loginDto = new LoginDTO{Email = "teste@email.com", Password = "123456"};

        var response1 = await _authService.Login(loginDto);
        var response2 = await _authService.Login(loginDto);

        Assert.NotNull(response1);
        Assert.NotNull(response2);

        Assert.NotEqual(response1.Data!.AccessToken, response2.Data!.AccessToken);
        Assert.NotEqual(response1.Data.RefreshToken, response2.Data.RefreshToken);

        _mockRedisService.Verify(r => r.SetAsync(It.IsAny<string>(), func.Id.ToString(), It.IsAny<TimeSpan>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Login_Should_Set_Access_Token_Expiration_Correctly()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByEmail(It.Is<string>(email => email == "teste@email.com")))
            .ReturnsAsync(func);

        var expiration = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
        _mockJwtService
            .Setup(j => j.GenerateAccessToken(It.IsAny<Funcionario>()))
            .Returns(("access-token", expiration));

        _mockJwtService
            .Setup(j => j.GenerateRefreshToken(It.IsAny<Funcionario>()))
            .Returns("refresh-token");

        _mockRedisService
            .Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

        var loginDto = new LoginDTO{Email = "teste@email.com", Password = "123456"};

        var response = await _authService.Login(loginDto);

        Assert.Equal(expiration, response.Data!.ExpiresAt);
    }

    [Fact]
    public async Task Logout_Should_Delete_Refresh_Token_When_Valid()
    {
        var refresh = "refresh-token";
        var id = Guid.NewGuid();

        _mockRedisService
            .Setup(r => r.GetAsync($"refresh:{refresh}"))
            .ReturnsAsync(id.ToString());

        _mockUserContextService
            .Setup(r => r.GetUserId())
            .Returns(id.ToString());

        _mockRedisService.Setup(r => r.DeleteAsync($"refresh:{refresh}"))
                   .Returns(Task.CompletedTask);

        await _authService.Logout("refresh-token");

        _mockRedisService.Verify(r =>
            r.DeleteAsync($"refresh:refresh-token"),
            Times.Once);
    }

    [Fact]
    public async Task Logout_Should_Throw_When_Refresh_Token_Not_Found()
    {
        _mockRedisService
            .Setup(r => r.GetAsync("refresh-token"))
            .ReturnsAsync((string?)null);

        _mockUserContextService
            .Setup(r => r.GetUserId())
            .Returns("123");

        _mockRedisService.Setup(r => r.DeleteAsync("refresh-token"))
                   .Returns(Task.CompletedTask);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.Logout("refresh-token")
        );

        Assert.Equal("Refresh token inválido", ex.Message);
    }

    [Fact]
    public async Task Logout_Should_Throw_When_Refresh_Token_Does_Not_Belong_To_User()
    {
        _mockRedisService
            .Setup(r => r.GetAsync("refresh-token"))
            .ReturnsAsync("123");

        _mockUserContextService
            .Setup(r => r.GetUserId())
            .Returns("456");

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.Logout("refresh-token")
        );

        Assert.Equal("Refresh token inválido", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Logout_Should_Throw_Exception_When_Token_Is_Null_Or_Empty(string invalidToken)
    {
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.Logout(invalidToken)
        );

        Assert.Equal("Refresh token inválido", exception.Message);
    }

    [Fact]
    public async Task Logout_Should_Throw_Exception_When_Redis_Fails()
    {
        var refreshToken = "refresh-token";

        _mockRedisService
            .Setup(r => r.GetAsync($"refresh:{refreshToken}"))
            .ThrowsAsync(new Exception("Redis falhou"));

        _mockUserContextService
            .Setup(r => r.GetUserId())
            .Returns("123");

        var ex = await Assert.ThrowsAsync<Exception>(() => _authService.Logout(refreshToken));

        Assert.Equal("Redis falhou", ex.Message);

        _mockRedisService.Verify(r => r.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Logout_Should_Throw_Exception_When_Redis_Delete_Fails()
    {
        var refreshToken = "refresh-token";

        _mockRedisService
            .Setup(r => r.GetAsync($"refresh:{refreshToken}"))
            .ReturnsAsync("123");

        _mockUserContextService
            .Setup(r => r.GetUserId())
            .Returns("123");

        _mockRedisService
            .Setup(r => r.DeleteAsync($"refresh:{refreshToken}"))
            .ThrowsAsync(new Exception("Falha ao deletar no Redis"));

        var ex = await Assert.ThrowsAsync<Exception>(() => _authService.Logout(refreshToken));

        Assert.Equal("Falha ao deletar no Redis", ex.Message);

        _mockRedisService.Verify(r => r.GetAsync($"refresh:{refreshToken}"), Times.Once);

        _mockRedisService.Verify(r => r.DeleteAsync($"refresh:{refreshToken}"), Times.Once);
    }

    [Fact]
    public async Task Refresh_Should_Return_Token_When_Refresh_Token_Is_Valid()
    {
        var refreshToken = "refresh-token";

        var func = Create_Funcionario();


        _mockRedisService
            .Setup(r => r.GetAsync($"refresh:{refreshToken}"))
            .ReturnsAsync(func.Id.ToString());

        _mockRedisService
            .Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        _mockRedisService.Setup(r => r.DeleteAsync($"refresh:{refreshToken}"))
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

        var response = await _authService.Refresh(refreshToken);

        Assert.NotNull(response);
        Assert.Equal("access-token", response.Data!.AccessToken);
        Assert.Equal("refresh-token", response.Data.RefreshToken);

        _mockRedisService.Verify(r =>
            r.DeleteAsync($"refresh:refresh-token"),
            Times.Once);

        _mockRedisService.Verify(r => r.SetAsync(
            $"refresh:refresh-token",
            func.Id.ToString(),
            It.IsAny<TimeSpan>()
        ), Times.Once);
    }

    [Fact]
    public async Task Refresh_Should_Throw_When_Refresh_Token_Not_Found()
    {
        _mockRedisService
            .Setup(r => r.GetAsync("refresh-token"))
            .ReturnsAsync((string?)null);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.Logout("refresh-token")
        );

        Assert.Equal("Refresh token inválido", ex.Message);
    }

    [Fact]
    public async Task Refresh_Should_Throw_When_User_NotFound()
    {
        _mockRedisService
           .Setup(r => r.GetAsync("refresh-token"))
           .ReturnsAsync("123");

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Funcionario?)null);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.Refresh("refresh-token")
        );

        Assert.Equal("Refresh token inválido", ex.Message);
    }

    [Fact]
    public async Task Refresh_Should_Throw_When_Redis_Set_Fails()
    {
        var refreshToken = "refresh-token";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        var userId = Guid.NewGuid();
        var func = Create_Funcionario();

        _mockRedisService.Setup(r => r.GetAsync($"refresh:{refreshToken}"))
            .ReturnsAsync(userId.ToString());

        _mockRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(func);

        _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<Funcionario>()))
            .Returns(("access-token", DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
        _mockJwtService.Setup(j => j.GenerateRefreshToken(It.IsAny<Funcionario>()))
            .Returns("new-refresh-token");

        _mockRedisService.Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ThrowsAsync(new Exception("Redis error"));

        await Assert.ThrowsAsync<Exception>(() => _authService.Refresh(refreshToken));
    }

    [Fact]
    public async Task Logout_Should_Set_Access_Token_Expiration_Correctly()
    {
        var refreshToken = "refresh-token";
        var func = Create_Funcionario();

        _mockRedisService.Setup(r => r.GetAsync($"refresh:{refreshToken}"))
            .ReturnsAsync(func.Id.ToString());

        _mockRepository.Setup(r => r.GetByIdAsync(func.Id))
            .ReturnsAsync(func);

        var expiration = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();

        _mockJwtService.Setup(j => j.GenerateAccessToken(It.IsAny<Funcionario>()))
            .Returns(("access-token", expiration));

        _mockJwtService.Setup(j => j.GenerateRefreshToken(It.IsAny<Funcionario>()))
            .Returns("new-refresh-token");

        var response = await _authService.Refresh(refreshToken);

        Assert.Equal(expiration, response.Data!.ExpiresAt);
    }
}