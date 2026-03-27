namespace GerenciadorFuncionarios.DTOs.Auth.Responses;

using GerenciadorFuncionarios.Enums;

public record AuthResponseDTO
    (
        string AccessToken,
        long ExpiresAt,
        string Email,
        Role Role
    )
{ }