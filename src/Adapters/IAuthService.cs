using GerenciadorFuncionarios.DTOs.Auth.Requests;
using GerenciadorFuncionarios.DTOs.Auth.Responses;
using GerenciadorFuncionarios.Shared.Responses;

namespace GerenciadorFuncionarios.Adapters;

public interface IAuthService
{
    Task<ApiResponse<TokenResponseDTO>> Login(LoginDTO data);
    Task Logout(string refreshToken);
    Task<ApiResponse<TokenResponseDTO>> Refresh(string refreshToken);
}