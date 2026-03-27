namespace GerenciadorFuncionarios.DTOs.Departamento.Requests;

using System.ComponentModel.DataAnnotations;

public record RegistrarDepartamentoDTO
    (
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100)]
        string Name
    )
{ }