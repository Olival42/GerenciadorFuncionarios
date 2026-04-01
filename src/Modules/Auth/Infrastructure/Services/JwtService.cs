namespace GerenciadorFuncionarios.Modules.Auth.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public (string accessToken, long expiresAt) GenerateAccessToken(Funcionario usuario)
    {
        throw new NotImplementedException();
    }

    public string GenerateRefreshToken(Funcionario usuario)
    {
        throw new NotImplementedException();
    }
}