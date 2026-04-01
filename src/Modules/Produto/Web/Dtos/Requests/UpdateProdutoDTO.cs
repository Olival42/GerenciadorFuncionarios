namespace GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;

using System;
using System.ComponentModel.DataAnnotations;

public record UpdateProdutoDTO
{
    [StringLength(100)]
    public string? Name { get; init; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que 0")]
    public decimal? Price { get; init; }
}