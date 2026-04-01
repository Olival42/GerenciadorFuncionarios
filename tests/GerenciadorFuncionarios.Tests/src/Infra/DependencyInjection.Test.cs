using Xunit;
using Microsoft.Extensions.Configuration;
using GerenciadorFuncionarios.Infra;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using GerenciadorFuncionarios.Services;
using GerenciadorFuncionarios.Adapters;
using GerenciadorFuncionarios.Services.Security;
using GerenciadorFuncionarios.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Moq;
using GerenciadorFuncionarios.Repositories;

public class DependencyInjectionTests
{
    private ServiceCollection CreateBaseServices()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddOptions();
        services.AddMemoryCache();
        services.AddHttpContextAccessor();

        services.AddSingleton<IConfiguration>(
            new ConfigurationBuilder().Build()
        );

        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        services.AddSingleton<IConnectionMultiplexer>(
            Mock.Of<IConnectionMultiplexer>()
        );

        return services;
    }

    [Fact]
    public void AddServices_Should_Register_Services()
    {
        var services = CreateBaseServices();

        services.AddRepositories();
        services.AddCache();
        services.AddSecurity();
        services.AddServices();

        var provider = services.BuildServiceProvider();

        Assert.IsType<FuncionarioService>(
             provider.GetService<IFuncionarioService>()
         );

        Assert.IsType<AuthService>(
            provider.GetService<IAuthService>()
        );
    }

    [Fact]
    public void AddRepositories_Should_Register_Repositories()
    {
        var services = CreateBaseServices();

        services.AddRepositories();

        var provider = services.BuildServiceProvider();

        Assert.IsType<FuncionarioRepository>(
            provider.GetService<IFuncionarioRepository>()
        );
    }

    [Fact]
    public void AddSecurity_Should_Register_Security()
    {
        var services = CreateBaseServices();

        services.AddSecurity();

        var provider = services.BuildServiceProvider();

        Assert.IsType<JwtService>(
            provider.GetService<IJwtService>()
        );

        Assert.IsType<UserContextService>(
            provider.GetService<IUserContextService>()
        );
    }

    [Fact]
    public void AddCache_Should_Register_Cache()
    {
        var services = CreateBaseServices();

        services.AddCache();

        var provider = services.BuildServiceProvider();

        Assert.IsType<RedisService>(
            provider.GetService<IRedisService>()
        );
    }
}