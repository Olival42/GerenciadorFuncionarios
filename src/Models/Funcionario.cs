namespace GerenciadorFuncionarios.Models;

using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

[Index(nameof(CPF), IsUnique = true)]
public class Funcionario : Usuario
{
    public required string Name { get; set; }

    public required string Phone { get; set; }

    public required string CPF { get; set; }

    public Guid? DepartamentoId { get; set; }

    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(DepartamentoId))]
    public Departamento? Departamento {  get; set; }
}
