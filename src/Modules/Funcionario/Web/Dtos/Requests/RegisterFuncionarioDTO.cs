namespace GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;

using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;

public record RegisterFuncionarioDTO {

    public required string Name { get; init; }

    public required string UserName { get; init; }

    public required string Password { get; init; }

    public required Role Role { get; init; }

    public required string Phone { get; init; }

    public required string CPF { get; init; }
}

