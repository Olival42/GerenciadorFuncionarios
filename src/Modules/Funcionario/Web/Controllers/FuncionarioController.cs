namespace GerenciadorFuncionarios.Modules.Funcionario.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GerenciadorFuncionarios.Modules.Funcionario.Application.Services;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;
using GerenciadorFuncionarios.Shared.Responses;

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

    [Authorize(Roles = "GERENTE")]
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

    [Authorize(Roles = "GERENTE")]
    [HttpDelete("{id}")]
    public Task<IActionResult> InactiveById(Guid id)
    {
        throw new NotImplementedException();
    }

    [Authorize(Roles = "GERENTE")]
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