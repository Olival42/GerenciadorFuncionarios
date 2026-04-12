namespace GerenciadorFuncionarios.Modules.Funcionario.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

[Produces("application/json")]
[ApiController]
[Route("api/funcionarios")]
public class FuncionarioController : ControllerBase
{
    private readonly IRegistrarFuncionario _registrarFuncionario;
    private readonly IObterFuncionarioLogado _obterFuncionarioLogado;
    private readonly IObterFuncionarioPorId _obterFuncionarioPorId;
    private readonly IInativarFuncionario _inativarFuncionario;
    private readonly IAtualizarFuncionario _atualizarFuncionario;
    private readonly IObterTodosFuncionarios _obterTodosFuncionarios;

    public FuncionarioController(
        IRegistrarFuncionario registrarFuncionario,
        IObterFuncionarioLogado obterFuncionarioLogado,
        IObterFuncionarioPorId obterFuncionarioPorId,
        IInativarFuncionario inativarFuncionario,
        IAtualizarFuncionario atualizarFuncionario,
        IObterTodosFuncionarios obterTodosFuncionarios)
    {
        _registrarFuncionario = registrarFuncionario;
        _obterFuncionarioLogado = obterFuncionarioLogado;
        _obterFuncionarioPorId = obterFuncionarioPorId;
        _inativarFuncionario = inativarFuncionario;
        _atualizarFuncionario = atualizarFuncionario;
        _obterTodosFuncionarios = obterTodosFuncionarios;
    }

    [Authorize(Roles = "GERENTE")]
    [HttpPost("registrar")]
    public async Task<ActionResult<ApiResponse<ResponseFuncionarioDTO>>> Resgister([FromBody] RegisterFuncionarioDTO data)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<ResponseFuncionarioDTO>>> GetMeAsync()
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ResponseFuncionarioDTO>>> GetFuncionarioById(Guid id)
    {
        throw new NotImplementedException();
    }

    [Authorize(Roles = "GERENTE")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> InactiveById(Guid id)
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
    public async Task<IActionResult> GetAllFuncionarios(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        throw new NotImplementedException();
    }
}