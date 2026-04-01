namespace GerenciadorFuncionarios.Services.Security;

using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Adapters;
using Microsoft.Extensions.Configuration;

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