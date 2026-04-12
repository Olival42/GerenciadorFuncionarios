using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Auth.Web.Responses;
using GerenciadorFuncionarios.Shared.Responses;

namespace GerenciadorFuncionarios.Modules.Auth.Application.UseCases;

public interface IRefresh
{
    Task<ApiResponse<TokenResponseDTO>> Execute(string refreshToken);
}
