namespace GerenciadorFuncionarios.Modules.Auth.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;
using GerenciadorFuncionarios.Modules.Auth.Web.Responses;
using GerenciadorFuncionarios.Modules.Auth.Web.Requests;
using GerenciadorFuncionarios.Shared.Responses;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
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