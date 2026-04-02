namespace GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;

public record UpdateEstoqueDTO
{
    public required int Quantity { get; init; }
}