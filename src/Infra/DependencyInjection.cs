namespace GerenciadorFuncionarios.Infra;

using GerenciadorFuncionarios.Adapters;
using GerenciadorFuncionarios.Ports;
using GerenciadorFuncionarios.Services;
using GerenciadorFuncionarios.Services.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<DepartamentoService>();
        services.AddScoped<IFuncionarioService, FuncionarioService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
        services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
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