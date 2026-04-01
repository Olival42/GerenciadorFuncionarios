using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;

namespace GerenciadorFuncionarios.Modules.Auth.Web.Responses;

public record TokenResponseDTO
    (
        string AccessToken,
        long ExpiresAt,
        string RefreshToken,
        string Email,
        Role Role
    )
{ }