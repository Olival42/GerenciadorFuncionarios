using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;

namespace GerenciadorFuncionarios.Modules.Auth.Web.Responses;

public record AuthResponseDTO
    (
        string AccessToken,
        long ExpiresAt,
        string Email,
        Role Role
    )
{ }