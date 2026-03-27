namespace GerenciadorFuncionarios.Infra;

using GerenciadorFuncionarios.Services;
using GerenciadorFuncionarios.Services.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<DepartamentoService>();
        services.AddScoped<FuncionarioService>();
        services.AddScoped<AuthService>();
        return services;
    }

    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddScoped<JwtService>();
        services.AddScoped<UserContextService>();
        return services;
    }

    public static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddScoped<RedisService>();
        return services;
    }
}