namespace GerenciadorFuncionarios.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Adapters;

[Produces("application/json")]
[ApiController]
[Route("api/funcionarios")]
public class FuncionarioController : ControllerBase
{
    private readonly IFuncionarioService _service;

    public FuncionarioController(IFuncionarioService service)
    {
        _service = service;
    }

    [Authorize(Roles = "ADMIN,GERENTE")]
    [HttpPost("registrar")]
    public Task<ActionResult<ApiResponse<ResponseFuncionarioDTO>>> Resgister([FromBody] RegisterFuncionarioDTO data)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpGet("{id}")]
    public Task<ActionResult<ApiResponse<ResponseFuncionarioDTO>>> GetFuncionarioById(Guid id)
    {
        throw new NotImplementedException();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id}")]
    public Task<IActionResult> InactiveById(Guid id)
    {
        throw new NotImplementedException();
    }

    [Authorize(Roles = "ADMIN,GERENTE")]
    [HttpPatch("{id}")]
    public Task<ActionResult<ApiResponse<ResponseFuncionarioDTO>>> UpdateFuncionario(Guid id, [FromBody] UpdateFuncionarioDTO data)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpGet]
    public Task<IActionResult> GetAllFuncionarios(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        throw new NotImplementedException();
    }
}