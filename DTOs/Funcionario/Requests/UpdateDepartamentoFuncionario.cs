namespace GerenciadorFuncionarios.DTOs.Funcionario.Requests;

using System;
using System.ComponentModel.DataAnnotations;

public record UpdateDepartamentoFuncionario 
    (
        [Required(ErrorMessage = "Departamento é obrigatório")]
        Guid DepartamentoId
    ) { }
