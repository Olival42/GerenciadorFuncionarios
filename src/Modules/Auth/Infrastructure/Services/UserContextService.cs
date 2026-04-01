namespace GerenciadorFuncionarios.Modules.Auth.Infrastructure.Services;

using Microsoft.AspNetCore.Http;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetUserId()
    {
        throw new NotImplementedException();
    }
}