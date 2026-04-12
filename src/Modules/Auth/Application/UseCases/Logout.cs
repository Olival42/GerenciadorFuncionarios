namespace GerenciadorFuncionarios.Modules.Auth.Application.UseCases;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Infrastructure.Cache.Interfaces;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;

public class Logout : ILogout
{
    private readonly IRedisService _redisService;
    private readonly IUserContextService _userContext;

    public Logout(IRedisService redisService, IUserContextService userContext)
    {
        _redisService = redisService;
        _userContext = userContext;
    }

    public async Task Execute(string refreshToken)
    {
        throw new NotImplementedException();
    }
}
