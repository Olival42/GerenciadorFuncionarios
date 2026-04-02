namespace GerenciadorFuncionarios.Modules.Produto.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GerenciadorFuncionarios.Modules.Produto.Application.Services;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;

[Produces("application/json")]
[ApiController]
[Route("api/produtos")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoService _service;

    public ProdutoController(IProdutoService service)
    {
        _service = service;
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
        var result = await _service.BaixarEstoque(id, data);
        return Ok(result);
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