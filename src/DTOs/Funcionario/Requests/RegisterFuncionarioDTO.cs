namespace GerenciadorFuncionarios.DTOs.Funcionario.Requests;

using System;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Validation.Attributes;

public record RegisterFuncionarioDTO
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required(ErrorMessage = "Telefone é obrigatório")]
    [RegularExpression(@"^\d{2}\d{4,5}\d{4}$", ErrorMessage = "Telefone inválido. Formato esperado: 99999999999")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "CPF é obrigatório")]
    [CPFValid]
    public string CPF { get; set; }

    [Required(ErrorMessage = "Departamento é obrigatório")]
    public Guid DepartamentoId { get; set; }
}
