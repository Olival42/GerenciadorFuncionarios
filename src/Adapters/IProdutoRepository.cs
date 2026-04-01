using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Shared.Responses;

namespace GerenciadorFuncionarios.Adapters;

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