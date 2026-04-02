namespace GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;

public record UpdateProdutoDTO
{
    public string? Name { get; init; }
    public double? Price { get; init; }
}