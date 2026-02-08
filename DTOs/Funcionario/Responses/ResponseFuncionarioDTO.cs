namespace GerenciadorFuncionarios.DTOs.Funcionario.Responses;

using System;

public record ResponseFuncionarioDTO 
    (
        Guid Id,
        string Name,
        string Phone,
        string Email,
        string CPF,
        Guid DepartamentoId
    )
{ }
