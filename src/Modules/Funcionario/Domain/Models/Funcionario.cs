namespace GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;

using System;
using Microsoft.EntityFrameworkCore;
using GerenciadorFuncionarios.Modules.Auth.Domain.Models;

[Index(nameof(CPF), IsUnique = true)]
public class Funcionario : Usuario
{
    public required string Name { get; set; }

    public required string Phone { get; set; }

    public required string CPF { get; set; }

    public bool IsActive { get; set; } = true;
}
