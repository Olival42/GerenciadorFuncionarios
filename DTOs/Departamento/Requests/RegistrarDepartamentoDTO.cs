namespace GerenciadorFuncionarios.DTOs.Departamento.Requests;

using System;
using System.ComponentModel.DataAnnotations;

public record RegistrarDepartamentoDTO
    (
        [Required(ErrorMessage = "Nome é obrrigatório")]
        [StringLength(100, MinimumLength = 3)]
        string Name
    )
{ }