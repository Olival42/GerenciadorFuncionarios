namespace GerenciadorFuncionarios.Modules.Produto.Application.Services;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public interface IProdutoService
{
    Task<ApiResponse<ResponseProdutoDTO>> RegistrarProdutoAsync(RegisterProdutoDTO data);
    Task<ApiResponse<ResponseProdutoDTO>> ObterProdutoPorId(Guid id);
    Task InativarPorId(Guid id);
    Task<ApiResponse<ResponseProdutoDTO>> Atualizar(Guid id, UpdateProdutoDTO data);
    Task<ApiResponse<ResponseProdutoDTO>> EntradaEstoque(Guid id, int quantidade);
    Task<ApiResponse<ResponseProdutoDTO>> BaixarEstoque(Guid id, int quantidade);
    Task<ApiResponse<PaginationResponse<ResponseProdutoDTO>>> ObterTodosFuncionarios(int page, int pageSize);
    Task VerificarEstoqueBaixoAsync(int limite = 5);
}