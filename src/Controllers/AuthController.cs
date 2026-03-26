using Microsoft.AspNetCore.Mvc;
using GerenciadorFuncionarios.DTOs.Auth.Requests;
using GerenciadorFuncionarios.DTOs.Auth.Responses;
using GerenciadorFuncionarios.Services;
using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
	private readonly AuthService _service;

	public AuthController(AuthService service)
	{
		_service = service;
	}

	[AllowAnonymous]
	[HttpPost("login")]
	public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login([FromBody] LoginDTO data)
	{
		var result = await _service.Login(data);

		Response.Cookies.Append(
			"refreshToken",
			result.Data!.RefreshToken,
			new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.UtcNow.AddDays(7),
			}
		);

		var response = ApiResponse<AuthResponseDTO>.Ok(
			new AuthResponseDTO(
				AccessToken: result.Data!.AccessToken,
				ExpiresAt: result.Data!.ExpiresAt,
				Email: result.Data!.Email,
				Role: result.Data!.Role
		));

		return Ok(response);
	}

	[Authorize]
	[HttpPost("logout")]
	public async Task<IActionResult> Logout()
	{
		var refreshToken = Request.Cookies["refreshToken"];

		if (!string.IsNullOrEmpty(refreshToken))
		{
			await _service.Logout(refreshToken);
		}

		Response.Cookies.Delete("refreshToken");

		return NoContent();
	}

	// Arruma 
	[AllowAnonymous]
	[HttpPost("refresh")]
	public async Task<IActionResult> Refresh()
	{
		var refreshToken = Request.Cookies["refreshToken"];

		if (string.IsNullOrEmpty(refreshToken))
			throw new UnauthorizedAccessException("Refresh token é obrigatório");

		var result = await _service.Refresh(refreshToken);

		Response.Cookies.Append(
			"refreshToken",
			result.Data!.RefreshToken,
			new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.UtcNow.AddDays(7),
			}
		);

		var response = ApiResponse<AuthResponseDTO>.Ok(
			new AuthResponseDTO(
				AccessToken: result.Data!.AccessToken,
				ExpiresAt: result.Data!.ExpiresAt,
				Email: result.Data!.Email,
				Role: result.Data!.Role
		));

		return Ok(response);
	}
}
