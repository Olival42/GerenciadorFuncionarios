namespace GerenciadorFuncionarios.Modules.Auth.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using GerenciadorFuncionarios.Modules.Auth.Web.Responses;
using GerenciadorFuncionarios.Modules.Auth.Web.Requests;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Auth.Application.UseCases;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogin _useCaseLogin;
    private readonly ILogout _useCaseLogout;
    private readonly IRefresh _useCaseRefresh;

    public AuthController(ILogin useCaseLogin, ILogout useCaseLogout, IRefresh useCaseRefresh)
    {
        _useCaseLogin = useCaseLogin;
        _useCaseLogout = useCaseLogout;
        _useCaseRefresh = useCaseRefresh;
    }

    [EnableRateLimiting("Auth")]
    [AllowAnonymous]
    [HttpPost("login")]
    public Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login([FromBody] LoginDTO data)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("logout")]
    public Task<IActionResult> Logout()
    {
        throw new NotImplementedException();
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public Task<IActionResult> Refresh()
    {
        throw new NotImplementedException();
    }
}