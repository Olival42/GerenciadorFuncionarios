namespace GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;

using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Validation.Attributtes;

public record UpdateFuncionarioDTO {
    [StringLength(100, ErrorMessage = "Nome tem que ter no máximo 100 caracteres")]
    public string? Name { get; init; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; init; }

    [StringLength(20, MinimumLength = 8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres e no máximo 20 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$", ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um dígito e um caractere especial.")]
    public string? Password { get; init; }

    [EnumDataType(typeof(Role), ErrorMessage = "Cargo inválido")]
    public Role? Role { get; init; }

    [RegularExpression(@"^\d{2}\d{4,5}\d{4}$", ErrorMessage = "Telefone inválido. Formato esperado: 99999999999")]
    public string? Phone { get; init; }

    [CPFValid]
    public string? CPF { get; init; }
}
