namespace GerenciadorFuncionarios.DTOs.Auth.Responses;

using GerenciadorFuncionarios.Enums;

public record TokenResponseDTO
    (
        string AccessToken,
        long ExpiresAt,
        string RefreshToken,
        string Email,
        Role Role
    )
{ }