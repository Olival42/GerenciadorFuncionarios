namespace GerenciadorFuncionarios.Infra;

using GerenciadorFuncionarios.Adapters;
using GerenciadorFuncionarios.Hubs;
using GerenciadorFuncionarios.Repositories;
using GerenciadorFuncionarios.Services;
using GerenciadorFuncionarios.Services.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IFuncionarioService, FuncionarioService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IEstoqueHub, EstoqueHub>();
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