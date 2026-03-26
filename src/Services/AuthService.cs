namespace GerenciadorFuncionarios.Services;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.Shared.Responses;
using BCrypt.Net;
using GerenciadorFuncionarios.DTOs.Auth.Requests;
using GerenciadorFuncionarios.Services.Security;
using GerenciadorFuncionarios.DTOs.Auth.Responses;
using GerenciadorFuncionarios.Exceptions;

public class AuthService
{
    private readonly AppDbContext _context;

    private readonly JwtService _jwtService;

    private readonly RedisService _redis;

    private readonly UserContextService _userContext;

    public AuthService(AppDbContext context, JwtService jwtService, RedisService redisService, UserContextService userContext)
    {
        _jwtService = jwtService;
        _context = context;
        _redis = redisService;
        _userContext = userContext;
    }

    public async Task<ApiResponse<TokenResponseDTO>> Login(LoginDTO data)
    {
        var func = await _context.Funcionario
            .Where(f => f.Email == data.Email)
            .FirstOrDefaultAsync();

        if (func == null || !BCrypt.Verify(data.Password, func!.PasswordHash))
        {
            throw new BadCredentialsException("Credenciais inválidas.");
        }

        var (accessToken, accessExpiresAt) = _jwtService.GenerateAccessToken(func);
        var refreshToken = _jwtService.GenerateRefreshToken(func);

        await _redis.SetAsync($"refresh:{refreshToken}", func.Id.ToString(), TimeSpan.FromDays(7));

        var dto = new TokenResponseDTO(
            AccessToken: accessToken,
            ExpiresAt: accessExpiresAt,
            RefreshToken: refreshToken,
            Email: func.Email,
            Role: func.Role
        );

        return ApiResponse<TokenResponseDTO>.Ok(dto);
    }

    public async Task Logout(string refreshToken)
    {
        var redisUserId = await _redis.GetAsync($"refresh:{refreshToken}");
        var contextUserId = _userContext.GetUserId();

        if (redisUserId != null && contextUserId == redisUserId)
        {
            await _redis.DeleteAsync($"refresh:{refreshToken}");
        }
    }

    public async Task<ApiResponse<TokenResponseDTO>> Refresh(string refreshToken)
    {
        var redisUserId = await _redis.GetAsync($"refresh:{refreshToken}");

        if(redisUserId == null)
        {
            throw new UnauthorizedAccessException("Refresh token inválido");
        }

        var func = await _context.Funcionario
                    .Where(f => f.Id.ToString() == redisUserId && f.IsActive)
                    .FirstOrDefaultAsync();

        if(func == null)
        {
            throw new UnauthorizedAccessException("Refresh token inválido");
        }

        await _redis.DeleteAsync($"refresh:{refreshToken}");

        var (accessToken, accessExpiresAt) = _jwtService.GenerateAccessToken(func);
        var newRefreshToken = _jwtService.GenerateRefreshToken(func);

        await _redis.SetAsync($"refresh:{newRefreshToken}", func.Id.ToString(), TimeSpan.FromDays(7));

        var dto = new TokenResponseDTO(
            AccessToken: accessToken,
            ExpiresAt: accessExpiresAt,
            RefreshToken: newRefreshToken,
            Email: func.Email,
            Role: func.Role
        );

        return ApiResponse<TokenResponseDTO>.Ok(dto);
    }
}
