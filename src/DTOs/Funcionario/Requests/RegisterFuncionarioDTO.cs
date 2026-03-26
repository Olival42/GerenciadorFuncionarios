namespace GerenciadorFuncionarios.DTOs.Funcionario.Requests;

using System;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Enums;
using GerenciadorFuncionarios.Validation.Attributes;

public record RegisterFuncionarioDTO
(
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100)]
    string Name,

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress]
    string Email,

    [Required(ErrorMessage = "Senha é obrigatório")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$", ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um dígito e um caractere especial.")]
    string Password,

    [Required(ErrorMessage = "Cargo é obrigatório")]
    [EnumDataType(typeof(Role))]
    Role Role,

    [Required(ErrorMessage = "Telefone é obrigatório")]
    [RegularExpression(@"^\d{2}\d{4,5}\d{4}$", ErrorMessage = "Telefone inválido. Formato esperado: 99999999999")]
    string Phone,

    [Required(ErrorMessage = "CPF é obrigatório")]
    [CPFValid]
    string CPF,

    [Required(ErrorMessage = "Departamento é obrigatório")]
    Guid DepartamentoId)
{ }

