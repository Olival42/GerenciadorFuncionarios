namespace GerenciadorFuncionarios.Modules.Produto.Application.Services;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public interface IProdutoService
{
    Task<ApiResponse<ResponseProdutoDTO>> RegistrarProdutoAsync(RegisterProdutoDTO data);
    Task<ApiResponse<ResponseProdutoDTO>> ObterProdutoPorId(Guid id);
    Task InativarPorId(Guid id);
    Task<ApiResponse<ResponseProdutoDTO>> Atualizar(Guid id, UpdateProdutoDTO data);
    Task<ApiResponse<ResponseProdutoDTO>> EntradaEstoque(Guid id, UpdateEstoqueDTO data);
    Task<ApiResponse<ResponseProdutoDTO>> BaixarEstoque(Guid id, UpdateEstoqueDTO data);
    Task<ApiResponse<PaginationResponse<ResponseProdutoDTO>>> ObterTodosProdutos(
    int page,
    int pageSize,
    string? name,
    decimal? priceMin,
    decimal? priceMax,
    TipoProduto? type);
    Task VerificarEstoqueBaixoAsync(int limite = 5);
}