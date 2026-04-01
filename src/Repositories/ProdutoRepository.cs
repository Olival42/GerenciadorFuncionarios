namespace GerenciadorFuncionarios.Repositories;

using GerenciadorFuncionarios.Adapters;
using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Shared.Responses;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Produto>> GetProdutosAtivosAsync()
    {
        // TODO: Retornar produtos ativos
        throw new NotImplementedException();
    }

    public async Task<Produto?> GetByIdAsync(Guid id)
    {
        // TODO: Buscar produto por id apenas ativo
        throw new NotImplementedException();
    }

    public async Task Add(Produto produto)
    {
        // TODO: Adicionar produto
        // TODO: Persistir no banco
        throw new NotImplementedException();
    }

    public async Task<bool> AnyByNameAsync(string name)
    {
        // TODO: Verificar existência por nome
        throw new NotImplementedException();
    }

    public async Task SaveChangesAsync()
    {
        // TODO: Persistir alterações
        throw new NotImplementedException();
    }

    public async Task<PaginationResponse<ResponseProdutoDTO>> GetAllAsync(
        int page,
        int pageSize)
    {
        // TODO: Buscar paginado
        // TODO: Mapear para DTO
        // TODO: Retornar PaginationResponse

        throw new NotImplementedException();
    }
}