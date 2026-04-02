namespace GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;

using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;

public record RegisterProdutoDTO
{
    public required string Name { get; init; }
    public required TipoProduto Type { get; init; }
    public required int Quantity { get; init; }
    public required double Price { get; init; }
}