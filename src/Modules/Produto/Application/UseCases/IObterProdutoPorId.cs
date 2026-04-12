using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

namespace GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

public interface IObterProdutoPorId
{
    Task<ApiResponse<ResponseProdutoDTO>> Execute(Guid id);
}