namespace GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;

using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;

public record UpdateFuncionarioDTO {
    public string? Name { get; init; }

    public string? UserName { get; init; }

    public string? Password { get; init; }

    public Role? Role { get; init; }

    public string? Phone { get; init; }

    public string? CPF { get; init; }
}
