namespace GerenciadorFuncionarios.Services.Security;

using System.Security.Claims;
using GerenciadorFuncionarios.Adapters;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetUserId()
    {
        return _httpContextAccessor.HttpContext?
            .User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}