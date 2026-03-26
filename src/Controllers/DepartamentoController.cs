using Microsoft.AspNetCore.Mvc;
using GerenciadorFuncionarios.DTOs.Departamento.Requests;
using GerenciadorFuncionarios.DTOs.Departamento.Response;
using GerenciadorFuncionarios.Services;
using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.AspNetCore.Authorization;


[ApiController]
[Route("api/departamentos")]
public class DepartamentoController : ControllerBase
{
	private readonly DepartamentoService _service;

	public DepartamentoController(DepartamentoService service)
	{
		_service = service;
	}

    [Authorize(Roles = "ADMIN")]
	[HttpPost("registrar")]
	public async Task<ActionResult<ApiResponse<ResponseDepartamentoDTO>>> Resgister([FromBody] RegistrarDepartamentoDTO data)
	{
		var result = await _service.RegistrarDepartamentoAsync(data);

        return CreatedAtAction(
		   nameof(GetDepartamentoById),
		   new { id = result.Data!.Id },
		   result
	   );
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ResponseDepartamentoDTO>>> GetDepartamentoById(Guid id)
    {
        var result = await _service.ObterDepartamentoPorId(id);

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> InactiveById(Guid id)
    {
        await _service.InativarDepartamento(id);

        return NoContent();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllDepartamentos(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.ObterTodosDepartamentos(page, pageSize);

        return Ok(result);
    }
}
