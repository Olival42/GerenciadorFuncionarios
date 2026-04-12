using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

namespace GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

public interface IObterTodosProdutos
{
    Task<ApiResponse<PaginationResponse<ResponseProdutoDTO>>> Execute(
        int page,
        int pageSize,
        string? name,
        decimal? priceMin,
        decimal? priceMax,
        TipoProduto? type);
}