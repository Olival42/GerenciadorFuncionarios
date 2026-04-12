using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Auth.Web.Requests;
using GerenciadorFuncionarios.Modules.Auth.Web.Responses;
using GerenciadorFuncionarios.Shared.Responses;

namespace GerenciadorFuncionarios.Modules.Auth.Application.UseCases;

public interface ILogin
{
    Task<ApiResponse<TokenResponseDTO>> Execute(LoginDTO data);
}
