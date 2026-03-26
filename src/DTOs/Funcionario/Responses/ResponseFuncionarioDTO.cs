namespace GerenciadorFuncionarios.DTOs.Funcionario.Responses;

using System;
using GerenciadorFuncionarios.Enums;

public record ResponseFuncionarioDTO 
    (
        Guid Id,
        string Name,
        string Phone,
        string Email,
        string CPF,
        Guid DepartamentoId,
        Role Role,
        bool IsActive
    )
{ }
