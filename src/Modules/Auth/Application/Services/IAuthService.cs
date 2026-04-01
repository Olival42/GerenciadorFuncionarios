namespace GerenciadorFuncionarios.Modules.Auth.Application.Services;

using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Auth.Web.Requests;
using GerenciadorFuncionarios.Modules.Auth.Web.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public interface IAuthService
{
    Task<ApiResponse<TokenResponseDTO>> Login(LoginDTO data);
    Task Logout(string refreshToken);
    Task<ApiResponse<TokenResponseDTO>> Refresh(string refreshToken);
}