namespace GerenciadorFuncionarios.Modules.Funcionario.Application.Services;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public interface IFuncionarioService
{
    Task<ApiResponse<ResponseFuncionarioDTO>> RegistrarFuncionarioAsync(RegisterFuncionarioDTO data);
    Task<ApiResponse<ResponseFuncionarioDTO>> ObterFuncionarioPorId(Guid id);
    Task InativarPorId(Guid id);
    Task<ApiResponse<ResponseFuncionarioDTO>> Atualizar(Guid id, UpdateFuncionarioDTO data);
    Task<ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>> ObterTodosFuncionarios(int page, int pageSize);
}