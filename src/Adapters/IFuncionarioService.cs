using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Shared.Responses;

namespace GerenciadorFuncionarios.Adapters;

public interface IFuncionarioService
{
    Task<ApiResponse<ResponseFuncionarioDTO>> RegistrarFuncionarioAsync(RegisterFuncionarioDTO data);
    Task<ApiResponse<ResponseFuncionarioDTO>> ObterFuncionarioPorId(Guid id);
    Task InativarPorId(Guid id);
    Task<ApiResponse<ResponseFuncionarioDTO>> Atualizar(Guid id, UpdateFuncionarioDTO data);
    Task<ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>> ObterTodosFuncionarios(int page, int pageSize);

}