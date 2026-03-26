using Microsoft.AspNetCore.Mvc;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Services;
using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.AspNetCore.Authorization;

[Produces("application/json")]
[ApiController]
[Route("api/funcionarios")]
public class FuncionarioController : ControllerBase
{
    private readonly FuncionarioService _service;

    public FuncionarioController(FuncionarioService service)
	{
        _service = service;
	}

    [Authorize(Roles = "ADMIN,GERENTE")]
    [HttpPost("registrar")]
    public async Task<ActionResult<ApiResponse<ResponseFuncionarioDTO>>> Resgister([FromBody] RegisterFuncionarioDTO data)
    {
        var result = await _service.RegistrarFuncionarioAsync(data);

        return CreatedAtAction(
           nameof(GetFuncionarioById),
           new { id = result.Data!.Id },
           result
       );
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ResponseFuncionarioDTO>>> GetFuncionarioById(Guid id)
    {
        var result = await _service.ObterFuncionarioPorId(id);

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> InactiveById(Guid id)
    {
        await _service.InativarPorId(id);

        return NoContent();
    }

    [Authorize(Roles = "ADMIN,GERENTE")]
    [HttpPatch("{id}")]
    public async Task<ActionResult<ApiResponse<ResponseFuncionarioDTO>>> UpdateFuncionario(Guid id, [FromBody] UpdateFuncionarioDTO data)
    {
        var result = await _service.Atualizar(id, data);

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,GERENTE")]
    [HttpPatch("{id}/departamento")]
    public async Task<ActionResult<ApiResponse<ResponseFuncionarioDTO>>> UpdateDepartamento(Guid id, [FromBody] UpdateDepartamentoFuncionario data)
    {
        var result = await _service.AtualizarDepartamento(id, data);

        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllFuncionarios(
        [FromQuery] Guid? departamentoId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.ObterTodosFuncionarios(page, pageSize, departamentoId);

        return Ok(result);
    }
}
