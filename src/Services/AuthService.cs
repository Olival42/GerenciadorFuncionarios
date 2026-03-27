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

    private readonly ILogger<AuthService> _logger;
    private readonly AppDbContext _context;

    private readonly JwtService _jwtService;

    private readonly RedisService _redis;

    private readonly UserContextService _userContext;

    public AuthService(AppDbContext context, JwtService jwtService, RedisService redisService, UserContextService userContext, ILogger<AuthService> logger)
    {
        _jwtService = jwtService;
        _context = context;
        _redis = redisService;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task<ApiResponse<TokenResponseDTO>> Login(LoginDTO data)
    {
        _logger.LogInformation(
            "Tentativa de login para Email: {Email}",
            data.Email
        );

        var func = await _context.Funcionario
            .Where(f => f.Email == data.Email)
            .FirstOrDefaultAsync();

        _logger.LogDebug(
            "Funcionario encontrado. Id: {Id} Email: {Email}",
            func?.Id,
            func?.Email);

        if (func == null || !BCrypt.Verify(data.Password, func!.PasswordHash))
        {
            _logger.LogWarning("Credenciais inválidas");
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

        _logger.LogInformation(
            "Login realizado com sucesso. UserId: {UserId} Email: {Email}",
            func.Id,
            func.Email
        );

        return ApiResponse<TokenResponseDTO>.Ok(dto);
    }

    public async Task Logout(string refreshToken)
    {
        _logger.LogInformation(
            "Tentativa de logout para refresh token: {refreshToken}",
            refreshToken[..8]
        );

        var redisUserId = await _redis.GetAsync($"refresh:{refreshToken}");
        var contextUserId = _userContext.GetUserId();

        _logger.LogInformation(
            "Ids coletados: redis={redisUserId} context={contextUserId}",
            redisUserId,
            contextUserId);

        if (redisUserId == null || contextUserId?.ToString() != redisUserId)
        {
            _logger.LogWarning("Refresh token inválido");
            throw new UnauthorizedAccessException("Refresh token inválido");
        }

        await _redis.DeleteAsync($"refresh:{refreshToken}");

        _logger.LogInformation(
            "Logout realizado com sucesso. UserId: {UserId}",
            contextUserId
        );
    }

    public async Task<ApiResponse<TokenResponseDTO>> Refresh(string refreshToken)
    {
        _logger.LogInformation(
            "Tentativa de refresh para refresh token: {refreshToken}",
            refreshToken[..8]
        );

        var redisUserId = await _redis.GetAsync($"refresh:{refreshToken}");

        _logger.LogInformation(
            "Id coletado: redis={redisUserId}",
            redisUserId);

        if (redisUserId == null)
        {
            _logger.LogWarning("Refresh token inválido");
            throw new UnauthorizedAccessException("Refresh token inválido");
        }

        var func = await _context.Funcionario
                    .Where(f => f.Id.ToString() == redisUserId && f.IsActive)
                    .FirstOrDefaultAsync();

        if (func == null)
        {
            _logger.LogWarning("Refresh token inválido");
            throw new UnauthorizedAccessException("Refresh token inválido");
        }

        var (accessToken, accessExpiresAt) = _jwtService.GenerateAccessToken(func);
        var newRefreshToken = _jwtService.GenerateRefreshToken(func);

        await _redis.SetAsync($"refresh:{newRefreshToken}", func.Id.ToString(), TimeSpan.FromDays(7));

        await _redis.DeleteAsync($"refresh:{refreshToken}");

        var dto = new TokenResponseDTO(
            AccessToken: accessToken,
            ExpiresAt: accessExpiresAt,
            RefreshToken: newRefreshToken,
            Email: func.Email,
            Role: func.Role
        );

        _logger.LogInformation(
            "Access token renovado com sucesso. UserId: {UserId}",
            func.Id
        );


        return ApiResponse<TokenResponseDTO>.Ok(dto);
    }
}
