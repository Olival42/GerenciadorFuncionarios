namespace GerenciadorFuncionarios.DTOs.Auth.Requests;

using System.ComponentModel.DataAnnotations;

public record LoginDTO {
        [Required(ErrorMessage = "Email é obrigatório")]
        public required string Email { get; init; }

        [Required(ErrorMessage = "Senha é obrigatório")]
        public required string Password { get; init; }
}