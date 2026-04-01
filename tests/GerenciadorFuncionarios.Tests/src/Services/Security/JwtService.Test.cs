using Xunit;
using Moq;
using GerenciadorFuncionarios.Services.Security;
using Microsoft.Extensions.Configuration;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class JwtServiceTests
{
    private readonly IConfiguration _config;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        var settings = new Dictionary<string, string>
        {
            {"Jwt:Key", "super-secret-key-123456789-super-secret-key"},
            {"Jwt:Issuer", "test"},
            {"Jwt:Audience", "test"}
        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();

        _jwtService = new JwtService(_config);
    }

    [Fact]
    public void GenerateAccessToken_Should_Return_Token()
    {
        var usuario = GetFuncionario();

        var result = _jwtService.GenerateAccessToken(usuario);

        Assert.False(string.IsNullOrEmpty(result.accessToken));
    }

    [Fact]
    public void GenerateAccessToken_Should_ContainUserIdClaim()
    {
        var user = GetFuncionario();

        var (token, _) = _jwtService.GenerateAccessToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var claim = jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);

        Assert.Equal(user.Id.ToString(), claim.Value);
    }

    [Fact]
    public void GenerateAccessToken_Should_ContainEmailClaim()
    {
        var user = GetFuncionario();

        var (token, _) = _jwtService.GenerateAccessToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var claim = jwt.Claims.First(c => c.Type == ClaimTypes.Name);

        Assert.Equal(user.Email, claim.Value);
    }

    [Fact]
    public void GenerateAccessToken_Should_ContainRoleClaim()
    {
        var user = GetFuncionario();

        var (token, _) = _jwtService.GenerateAccessToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var claim = jwt.Claims.First(c => c.Type == ClaimTypes.Role);

        Assert.Equal(user.Role.ToString(), claim.Value);
    }

    [Fact]
    public void GenerateAccessToken_Should_ContainTokenTypeAccess()
    {
        var user = GetFuncionario();

        var (token, _) = _jwtService.GenerateAccessToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var claim = jwt.Claims.First(c => c.Type == "token-type");

        Assert.Equal("access", claim.Value);
    }

    [Fact]
    public void GenerateAccessToken_Should_SetExpiration()
    {
        var user = GetFuncionario();

        var (token, _) = _jwtService.GenerateAccessToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var diff = jwt.ValidTo - DateTime.UtcNow;

        Assert.True(diff.TotalMinutes <= 30.5);
        Assert.True(diff.TotalMinutes >= 29.5);
    }

    [Fact]
    public void GenerateAccessToken_Should_ReturnExpiration()
    {
        var user = GetFuncionario();

        var (_, expiration) = _jwtService.GenerateAccessToken(user);

        Assert.True(expiration > 0);
    }

    [Fact]
    public void GenerateAccessToken_Should_SetIssuer()
    {
        var user = GetFuncionario();

        var (token, _) = _jwtService.GenerateAccessToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(_config["Jwt:Issuer"], jwt.Issuer);
    }

    [Fact]
    public void GenerateAccessToken_Should_SetAudience()
    {
        var user = GetFuncionario();

        var (token, _) = _jwtService.GenerateAccessToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(_config["Jwt:Audience"], jwt.Audiences.First());
    }

    [Fact]
    public void GenerateRefreshToken_Should_Return_Token()
    {
        var usuario = GetFuncionario();

        var result = _jwtService.GenerateRefreshToken(usuario);

        Assert.False(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void GenerateRefreshToken_Should_ContainUserIdClaim()
    {
        var user = GetFuncionario();

        var token = _jwtService.GenerateRefreshToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var claim = jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);

        Assert.Equal(user.Id.ToString(), claim.Value);
    }

    [Fact]
    public void GenerateRefreshToken_Should_ContainEmailClaim()
    {
        var user = GetFuncionario();

        var token = _jwtService.GenerateRefreshToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var claim = jwt.Claims.First(c => c.Type == ClaimTypes.Name);

        Assert.Equal(user.Email, claim.Value);
    }

    [Fact]
    public void GenerateRefreshToken_Should_ContainTokenTypeRefresh()
    {
        var user = GetFuncionario();

        var token = _jwtService.GenerateRefreshToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var claim = jwt.Claims.First(c => c.Type == "token-type");

        Assert.Equal("refresh", claim.Value);
    }

    [Fact]
    public void GenerateRefreshToken_Should_ContainRoleClaim()
    {
        var user = GetFuncionario();

        var token = _jwtService.GenerateRefreshToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.DoesNotContain(jwt.Claims, c => c.Type == ClaimTypes.Role);
    }

    [Fact]
    public void GenerateRefreshToken_Should_Expire_In_7_Days()
    {
        var user = GetFuncionario();

        var token = _jwtService.GenerateRefreshToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var expected = DateTime.UtcNow.AddDays(7);

        Assert.InRange(jwt.ValidTo, expected.AddSeconds(-5), expected.AddSeconds(5));
    }

    [Fact]
    public void GenerateRefreshToken_Should_SetIssuer()
    {
        var user = GetFuncionario();

        var token = _jwtService.GenerateRefreshToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(_config["Jwt:Issuer"], jwt.Issuer);
    }

    [Fact]
    public void GenerateRefreshToken_Should_SetAudience()
    {
        var user = GetFuncionario();

        var token = _jwtService.GenerateRefreshToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(_config["Jwt:Audience"], jwt.Audiences.First());
    }

    [Fact]
    public void Access_And_Refresh_Should_Be_Different()
    {
        var usuario = GetFuncionario();

        var access = _jwtService.GenerateAccessToken(usuario);
        var refresh = _jwtService.GenerateRefreshToken(usuario);

        Assert.NotEqual(access.accessToken, refresh);
    }

    [Fact]
    public void GenerateAccessToken_Should_Have_Valid_Signature()
    {
        var user = GetFuncionario();

        var (token, _) = _jwtService.GenerateAccessToken(user);

        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,

            ClockSkew = TimeSpan.Zero
        };

        var handler = new JwtSecurityTokenHandler();

        var principal = handler.ValidateToken(token, parameters, out _);

        Assert.NotNull(principal);
    }

    [Fact]
    public void GenerateRefreshToken_Should_Have_Valid_Signature()
    {
        var user = GetFuncionario();

        var token = _jwtService.GenerateRefreshToken(user);

        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateIssuer = true,
            ValidIssuer = _config["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = _config["Jwt:Audience"],

            ValidateLifetime = false
        };

        var handler = new JwtSecurityTokenHandler();

        var principal = handler.ValidateToken(token, parameters, out _);

        Assert.NotNull(principal);
    }

    [Fact]
    public void Token_Should_Fail_When_Modified()
    {
        var user = GetFuncionario();

        var (token, _) = _jwtService.GenerateAccessToken(user);

        token = token.Replace("a", "b");

        var handler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        Assert.ThrowsAny<Exception>(() =>
            handler.ValidateToken(token, parameters, out _));
    }

    private Funcionario GetFuncionario()
    {
        return new Funcionario
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Phone = "44999999999",
            CPF = "68714247097",
            Email = "teste@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = Role.GERENTE,
            IsActive = true
        };
    }
}