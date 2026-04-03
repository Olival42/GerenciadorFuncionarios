namespace GerenciadorFuncionarios.Modules.Auth.Web.Requests;

using System.ComponentModel.DataAnnotations;

public record LoginDTO {
        [Required(ErrorMessage = "Nome do usuário é obrigatório")]
        public required string UserName { get; init; }

        [Required(ErrorMessage = "Senha é obrigatório")]
        public required string Password { get; init; }
}