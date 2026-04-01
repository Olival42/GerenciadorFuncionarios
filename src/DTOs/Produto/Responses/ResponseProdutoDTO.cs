namespace GerenciadorFuncionarios.DTOs.Funcionario.Responses;

using GerenciadorFuncionarios.Enums;

public record ResponseProdutoDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public TipoProduto Type { get; init; }
    public int Quantity { get; init; }
    public double Price { get; init; }
    public bool IsActive { get; init; }
}