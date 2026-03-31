namespace GerenciadorFuncionarios.Adapters;

using GerenciadorFuncionarios.Models;

public interface IJwtService
{
    (string accessToken, long expiresAt) GenerateAccessToken(Funcionario usurios);
    string GenerateRefreshToken(Funcionario user);
}