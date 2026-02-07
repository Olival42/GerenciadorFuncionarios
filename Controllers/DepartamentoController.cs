using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GerenciadorFuncionarios.DTOs.Departamento.Requests;
using GerenciadorFuncionarios.DTOs.Departamento.Response;
using GerenciadorFuncionarios.Services;
using GerenciadorFuncionarios.Shared.Responses;

[ApiController]
[Route("api/departamentos")]
public class DepartamentoController : ControllerBase
{
	private readonly DepartamentoService _service;

	public DepartamentoController(DepartamentoService service)
	{
		_service = service;
	}

	[HttpPost("registrar")]
	public async Task<ActionResult<ApiResponse<ResponseDepartamentoDTO>>> Resgister([FromBody] RegistrarDepartamentoDTO data)
	{
		var result = await _service.RegistrarDepartamentoAsync(data);

        return CreatedAtAction(
		   nameof(GetDeaprtamentoById),
		   new { id = result.Data!.Id },
		   result
	   );
    }

    [HttpGet("{id}")]
    public ActionResult<ApiResponse<ResponseDepartamentoDTO>> GetDeaprtamentoById(Guid id)
    {
        var result = _service.ObterDepartamentoPorId(id);

        return Ok(result);
    }
}
