using GerenciadorFuncionarios.Enums;

namespace GerenciadorFuncionarios.DTOs.Auth.Responses;

public record TokenResponseDTO
    (
        string AccessToken,
        long ExpiresAt,
        string RefreshToken,
        string Email,
        Role Role
    )
{ }