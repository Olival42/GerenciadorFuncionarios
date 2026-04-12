namespace GerenciadorFuncionarios.Modules.Produto.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GerenciadorFuncionarios.Modules.Produto.Application.UseCases;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;

[Produces("application/json")]
[ApiController]
[Route("api/produtos")]
public class ProdutoController : ControllerBase
{
    private readonly IRegistrarProduto _registrarProduto;
    private readonly IObterProdutoPorId _obterProdutoPorId;
    private readonly IInativarProduto _inativarProduto;
    private readonly IAtualizarProduto _atualizarProduto;
    private readonly IEntradaEstoque _entradaEstoque;
    private readonly IBaixarEstoque _baixarEstoque;
    private readonly IObterTodosProdutos _obterTodosProdutos;

    public ProdutoController(
        IRegistrarProduto registrarProduto,
        IObterProdutoPorId obterProdutoPorId,
        IInativarProduto inativarProduto,
        IAtualizarProduto atualizarProduto,
        IEntradaEstoque entradaEstoque,
        IBaixarEstoque baixarEstoque,
        IObterTodosProdutos obterTodosProdutos)
    {
        _registrarProduto = registrarProduto;
        _obterProdutoPorId = obterProdutoPorId;
        _inativarProduto = inativarProduto;
        _atualizarProduto = atualizarProduto;
        _entradaEstoque = entradaEstoque;
        _baixarEstoque = baixarEstoque;
        _obterTodosProdutos = obterTodosProdutos;
    }

    [Authorize]
    [HttpPost("registrar")]
    public async Task<ActionResult<ApiResponse<ResponseProdutoDTO>>> Resgister([FromBody] RegisterProdutoDTO data)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ResponseProdutoDTO>>> GetProdutoById(Guid id)
    {
        throw new NotImplementedException();
    }

    [Authorize(Roles = "GERENTE")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> InactiveById(Guid id)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPatch("{id}")]
    public async Task<ActionResult<ApiResponse<ResponseProdutoDTO>>> UpdateProduto(Guid id, [FromBody] UpdateProdutoDTO data)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("{id}/entrada")]
    public async Task<ActionResult<ApiResponse<ResponseProdutoDTO>>> EntradaEstoque(
    Guid id,
    [FromBody] UpdateEstoqueDTO data)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{id:guid}/saida")]
    public async Task<IActionResult> SaidaEstoque(Guid id, [FromBody] UpdateEstoqueDTO data)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllProdutos(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? name = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] TipoProduto? tipo = null)
    {
        throw new NotImplementedException();
    }
}