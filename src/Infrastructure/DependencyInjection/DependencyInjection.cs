using GerenciadorFuncionarios.Infrastructure.Cache;
using GerenciadorFuncionarios.Infrastructure.Cache.Interfaces;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;
using GerenciadorFuncionarios.Modules.Auth.Application.UseCases;
using GerenciadorFuncionarios.Modules.Auth.Infrastructure.Services;
using GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Infrastructure.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Application.Services;
using GerenciadorFuncionarios.Modules.Produto.Application.UseCases;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Infrastructure.Repositories;

namespace GerenciadorFuncionarios.Infrastructure.DependencyInjection;


public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IRegistrarFuncionario, RegistrarFuncionario>();
        services.AddScoped<IObterFuncionarioLogado, ObterFuncionarioLogado>();
        services.AddScoped<IObterFuncionarioPorId, ObterFuncionarioPorId>();
        services.AddScoped<IInativarFuncionario, InativarFuncionario>();
        services.AddScoped<IAtualizarFuncionario, AtualizarFuncionario>();
        services.AddScoped<IObterTodosFuncionarios, ObterTodosFuncionarios>();
        services.AddScoped<IRegistrarProduto, RegistrarProduto>();
        services.AddScoped<IObterProdutoPorId, ObterProdutoPorId>();
        services.AddScoped<IInativarProduto, InativarProduto>();
        services.AddScoped<IAtualizarProduto, AtualizarProduto>();
        services.AddScoped<IEntradaEstoque, EntradaEstoque>();
        services.AddScoped<IBaixarEstoque, BaixarEstoque>();
        services.AddScoped<ILogin, Login>();
        services.AddScoped<ILogout, Logout>();
        services.AddScoped<IRefresh, Refresh>();
        services.AddScoped<IObterTodosProdutos, ObterTodosProdutos>();
        services.AddScoped<IEstoqueService, EstoqueService>();
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