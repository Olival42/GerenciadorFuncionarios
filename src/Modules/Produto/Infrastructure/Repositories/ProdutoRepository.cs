using GerenciadorFuncionarios.Infrastructure;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;

namespace GerenciadorFuncionarios.Modules.Produto.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<GerenciadorFuncionarios.Modules.Produto.Domain.Models.Produto>> GetProdutosAtivosAsync()
    {
        // TODO: Retornar produtos ativos
        throw new NotImplementedException();
    }

    public async Task<GerenciadorFuncionarios.Modules.Produto.Domain.Models.Produto?> GetByIdAsync(Guid id)
    {
        // TODO: Buscar produto por id apenas ativo
        throw new NotImplementedException();
    }

    public async Task Add(GerenciadorFuncionarios.Modules.Produto.Domain.Models.Produto produto)
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
        int pageSize,
        string? name,
        decimal? priceMin,
        decimal? priceMax,
        TipoProduto? type)
    {
        // TODO: Buscar paginado
        // TODO: Mapear para DTO
        // TODO: Retornar PaginationResponse

        throw new NotImplementedException();
    }

    public async Task<IEnumerable<GerenciadorFuncionarios.Modules.Produto.Domain.Models.Produto>> GetLowStockAsync(int limite = 10)
    {
        throw new NotImplementedException();
    }
}