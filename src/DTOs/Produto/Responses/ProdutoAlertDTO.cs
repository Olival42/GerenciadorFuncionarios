namespace GerenciadorFuncionarios.DTOs.Funcionario.Responses;

public record ProdutoAlertaDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public int Quantity { get; init; }
    public int LimiteMinimo { get; init; }
}