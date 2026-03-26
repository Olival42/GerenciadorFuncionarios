namespace GerenciadorFuncionarios.DTOs.Auth.Requests;

using System.ComponentModel.DataAnnotations;

public record LoginDTO
    (
        [Required(ErrorMessage = "Email é obrigatório")]
        string Email,

        [Required(ErrorMessage = "Senha é obrigatório")]
        string Password
    )
{ }