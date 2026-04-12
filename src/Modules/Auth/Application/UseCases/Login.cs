namespace GerenciadorFuncionarios.Modules.Auth.Application.UseCases;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;
using GerenciadorFuncionarios.Infrastructure.Cache.Interfaces;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Auth.Web.Responses;
using GerenciadorFuncionarios.Modules.Auth.Web.Requests;

public class Login : ILogin
{
    private readonly IFuncionarioRepository _repository;
    private readonly IJwtService _jwtService;
    private readonly IRedisService _redisService;

    public Login(IFuncionarioRepository repository, IJwtService jwtService, IRedisService redisService)
    {
        _repository = repository;
        _jwtService = jwtService;
        _redisService = redisService;
    }

    public async Task<ApiResponse<TokenResponseDTO>> Execute(LoginDTO data)
    {
        throw new NotImplementedException();
    }
}
