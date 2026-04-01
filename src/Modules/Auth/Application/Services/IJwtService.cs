namespace GerenciadorFuncionarios.Modules.Auth.Application.Services;

using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;

public interface IJwtService
{
    (string accessToken, long expiresAt) GenerateAccessToken(Funcionario usuario);
    string GenerateRefreshToken(Funcionario usuario);
}