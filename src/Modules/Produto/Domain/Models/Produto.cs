namespace GerenciadorFuncionarios.Modules.Produto.Domain.Models;

using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;

[Index(nameof(Name), IsUnique = true)]
public class Produto
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    public required TipoProduto Type { get; set; }

    public required int Quantity { get; set; }

    public required double Price { get; set; }

    public bool IsActive { get; set; } = true;
}