namespace GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;

using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Validation.Attributtes;

public record RegisterFuncionarioDTO {

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100)]
    public required string Name { get; init; }

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; init; }

    [Required(ErrorMessage = "Senha é obrigatório")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$", ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um dígito e um caractere especial.")]
    public required string Password { get; init; }

    [Required(ErrorMessage = "Cargo é obrigatório")]
    [EnumDataType(typeof(Role), ErrorMessage = "Cargo inválido")]
    public required Role Role { get; init; }

    [Required(ErrorMessage = "Telefone é obrigatório")]
    [RegularExpression(@"^\d{2}\d{4,5}\d{4}$", ErrorMessage = "Telefone inválido. Formato esperado: 99999999999")]
    public required string Phone { get; init; }

    [Required(ErrorMessage = "CPF é obrigatório")]
    [CPFValid]
    public required string CPF { get; init; }
}

