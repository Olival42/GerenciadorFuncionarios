namespace GerenciadorFuncionarios.DTOs.Funcionario.Requests;

using System;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Enums;

public record RegisterProdutoDTO
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100)]
    public required string Name { get; init; }

    [Required(ErrorMessage = "Tipo do produto é obrigatório")]
    [EnumDataType(typeof(TipoProduto), ErrorMessage = "Tipo do produto inválido")]
    public required TipoProduto Type { get; init; }

    [Required(ErrorMessage = "Quantidade é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que 0")]
    public required int Quantity { get; init; }

    [Required(ErrorMessage = "Preço é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que 0")]
    public required double Price { get; init; }
}