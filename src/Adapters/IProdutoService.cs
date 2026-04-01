using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Shared.Responses;

namespace GerenciadorFuncionarios.Adapters;

public interface IProdutoService
{
    Task<ApiResponse<ResponseProdutoDTO>> RegistrarProdutoAsync(RegisterProdutoDTO data);
    Task<ApiResponse<ResponseProdutoDTO>> ObterProdutoPorId(Guid id);
    Task InativarPorId(Guid id);
    Task<ApiResponse<ResponseProdutoDTO>> Atualizar(Guid id, UpdateProdutoDTO data);
    Task<ApiResponse<PaginationResponse<ResponseProdutoDTO>>> ObterTodosFuncionarios(int page, int pageSize);
    Task VerificarEstoqueBaixoAsync(int limite = 5);
    Task<ApiResponse<ResponseProdutoDTO>> EntradaEstoque(Guid id, int quantidade);
    Task<ApiResponse<ResponseProdutoDTO>> BaixarEstoque(Guid id, int quantidade);
}