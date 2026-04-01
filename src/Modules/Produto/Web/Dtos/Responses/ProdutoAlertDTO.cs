namespace GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;

public record ProdutoAlertaDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public int Quantity { get; init; }
    public int LimiteMinimo { get; init; }
}