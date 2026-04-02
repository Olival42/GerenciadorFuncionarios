namespace GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;

using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public interface IProdutoRepository
{
    Task<Produto?> GetByIdAsync(Guid id);
    Task Add(Produto produto);
    Task<bool> AnyByNameAsync(string name);
    Task SaveChangesAsync();
    Task<IQueryable<Produto>> GetProdutosAtivosAsync();
    Task<IEnumerable<Produto>> GetLowStockAsync(int limite = 10);
    Task<PaginationResponse<ResponseProdutoDTO>> GetAllAsync(
    int page,
    int pageSize,
    string? name,
    decimal? priceMin,
    decimal? priceMax,
    TipoProduto? type);
}