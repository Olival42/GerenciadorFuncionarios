namespace GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public interface IObterTodosFuncionarios
{
    Task<ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>> Execute(int page, int pageSize);
}