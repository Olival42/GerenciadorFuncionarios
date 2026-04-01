using GerenciadorFuncionarios.Infrastructure.Cache;
using GerenciadorFuncionarios.Infrastructure.Cache.Interfaces;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;
using GerenciadorFuncionarios.Modules.Auth.Web.Requests;
using GerenciadorFuncionarios.Modules.Auth.Web.Responses;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Shared.Responses;

namespace GerenciadorFuncionarios.Modules.Auth.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IFuncionarioRepository _repository;
    private readonly IJwtService _jwtService;
    private readonly IRedisService _redis;
    private readonly IUserContextService _userContext;

    public AuthService(
        IFuncionarioRepository repository,
        IJwtService jwtService,
        IRedisService redisService,
        IUserContextService userContext,
        ILogger<AuthService> logger)
    {
        _repository = repository;
        _jwtService = jwtService;
        _redis = redisService;
        _userContext = userContext;
        _logger = logger;
    }

    public Task<ApiResponse<TokenResponseDTO>> Login(LoginDTO data)
    {
        throw new NotImplementedException();
    }

    public Task Logout(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<TokenResponseDTO>> Refresh(string refreshToken)
    {
        throw new NotImplementedException();
    }
}