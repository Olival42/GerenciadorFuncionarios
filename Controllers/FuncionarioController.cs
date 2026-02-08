using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Services;
using GerenciadorFuncionarios.Shared.Responses;

[ApiController]
[Route("api/funcionarios")]
public class FuncionarioController : ControllerBase
{
    private readonly FuncionarioService _service;

    public FuncionarioController(FuncionarioService service)
	{
        _service = service;
	}

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

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ResponseFuncionarioDTO>>> GetFuncionarioById(Guid id)
    {
        var result = await _service.ObterFuncionarioPorId(id);

        return Ok(result);
    }
}
