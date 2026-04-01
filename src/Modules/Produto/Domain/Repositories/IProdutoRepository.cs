namespace GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;

using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public interface IProdutoRepository
{
    Task<Produto?> GetByIdAsync(Guid id);
    Task Add(Produto produto);
    Task<bool> AnyByNameAsync(string name);
    Task SaveChangesAsync();
    Task<PaginationResponse<ResponseProdutoDTO>> GetAllAsync(
        int page, int pageSize);
    Task<IQueryable<Produto>> GetProdutosAtivosAsync();
}