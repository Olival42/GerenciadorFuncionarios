namespace GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;

using System;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;

public record ResponseFuncionarioDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public string UserName { get; init; } = default!;
    public string CPF { get; init; } = default!;
    public Role Role { get; init; }
    public bool IsActive { get; init; }

}

