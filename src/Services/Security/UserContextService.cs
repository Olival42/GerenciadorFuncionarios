namespace GerenciadorFuncionarios.Services.Security;

using GerenciadorFuncionarios.Adapters;
using Microsoft.AspNetCore.Http;

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