using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciadorFuncionarios.Models;

[Table("funcionarios")]
[Index(nameof(CPF), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class Funcionario
{
	[Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string CPF { get; set; }

    public Guid DepartamentoId { get; set; }

    [ForeignKey(nameof(DepartamentoId))]
    public Departamento? Departamento {  get; set; }

}
