namespace GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;

using System.ComponentModel.DataAnnotations;

public record UpdateEstoqueDTO
{
    [Required(ErrorMessage = "Quantidade é obrigatório")]
    public required int Quantity { get; init; }
}