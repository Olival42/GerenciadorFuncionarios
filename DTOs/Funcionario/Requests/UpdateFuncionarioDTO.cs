namespace GerenciadorFuncionarios.DTOs.Funcionario.Requests;

using System;
using System.ComponentModel.DataAnnotations;

public record UpdateFuncionarioDTO
    (
        [StringLength(100)]
        string? Name,

        [RegularExpression(@"^\d{2}\d{4,5}\d{4}$", ErrorMessage = "Telefone inválido. Formato esperado: 99999999999")]
        string? Phone,

        [EmailAddress(ErrorMessage = "Email inválido")]
        string? Email

    ) { }
