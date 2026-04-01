using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;

namespace GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;


public record ResponseProdutoDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public TipoProduto Type { get; init; }
    public int Quantity { get; init; }
    public double Price { get; init; }
    public bool IsActive { get; init; }
}