namespace GerenciadorFuncionarios.Controllers;

using Microsoft.AspNetCore.Mvc;
using GerenciadorFuncionarios.DTOs.Auth.Requests;
using GerenciadorFuncionarios.DTOs.Auth.Responses;
using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using GerenciadorFuncionarios.Adapters;

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