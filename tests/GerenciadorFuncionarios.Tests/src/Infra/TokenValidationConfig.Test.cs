using Xunit;
using Microsoft.Extensions.Configuration;
using GerenciadorFuncionarios.Infra;
using Microsoft.IdentityModel.Tokens;

public class TokenValidationConfigTests
{
    [Fact]
    public void GetTokenValidationParameters_Should_Return_Valid_Config()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Issuer", "test-issuer" },
            { "Jwt:Audience", "test-audience" },
            { "Jwt:Key", "super-secret-key-123456789" }
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var result = TokenValidationConfig.GetTokenValidationParameters(config);

        Assert.True(result.ValidateIssuer);
        Assert.True(result.ValidateAudience);
        Assert.True(result.ValidateLifetime);
        Assert.True(result.ValidateIssuerSigningKey);
        Assert.True(result.RequireExpirationTime);

        Assert.Equal(TimeSpan.Zero, result.ClockSkew);

        Assert.Equal("test-issuer", result.ValidIssuer);
        Assert.Equal("test-audience", result.ValidAudience);

        Assert.NotNull(result.IssuerSigningKey);
        Assert.IsType<SymmetricSecurityKey>(result.IssuerSigningKey);
    }
}