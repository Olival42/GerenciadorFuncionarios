using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciadorFuncionarios.Models;

[Table("departamentos")]
public class Departamento
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public ICollection<Funcionario>? Funcionarios { get; set; } = new List<Funcionario>();

}
