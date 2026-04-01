using GerenciadorFuncionarios.Hubs;
using GerenciadorFuncionarios.Infrastructure.Cache;
using GerenciadorFuncionarios.Infrastructure.Cache.Interfaces;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;
using GerenciadorFuncionarios.Modules.Auth.Infrastructure.Services;
using GerenciadorFuncionarios.Modules.Funcionario.Application.Services;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Infrastructure.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Infrastructure.Services;
using GerenciadorFuncionarios.Modules.Produto.Application.Services;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Infrastructure.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Infrastructure.Services;

namespace GerenciadorFuncionarios.Infrastructure.DependencyInjection;


public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IFuncionarioService, FuncionarioService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<EstoqueHub>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        return services;
    }

    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUserContextService, UserContextService>();
        return services;
    }

    public static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddScoped<IRedisService, RedisService>();
        return services;
    }
}