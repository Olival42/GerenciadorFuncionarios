
using GerenciadorFuncionarios.Enums;

namespace GerenciadorFuncionarios.DTOs.Auth.Responses;

public record AuthResponseDTO
    (
        string AccessToken,
        long ExpiresAt,
        string Email,
        Role Role
    )
{ }