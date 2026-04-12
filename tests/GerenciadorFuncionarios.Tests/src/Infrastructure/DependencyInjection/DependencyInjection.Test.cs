using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Moq;
using GerenciadorFuncionarios.Infrastructure;
using GerenciadorFuncionarios.Infrastructure.Cache;
using GerenciadorFuncionarios.Infrastructure.DependencyInjection;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;
using GerenciadorFuncionarios.Modules.Funcionario.Infrastructure.Repositories;
using GerenciadorFuncionarios.Modules.Auth.Infrastructure.Services;
using GerenciadorFuncionarios.Modules.Produto.Infrastructure.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Infrastructure.Cache.Interfaces;

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

        Assert.IsType<ProdutoRepository>(
            provider.GetService<IProdutoRepository>()
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