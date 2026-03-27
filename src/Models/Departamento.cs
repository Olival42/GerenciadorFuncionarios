namespace GerenciadorFuncionarios.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("departamentos")]
public class Departamento
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Funcionario>? Funcionarios { get; set; } = new List<Funcionario>();

}