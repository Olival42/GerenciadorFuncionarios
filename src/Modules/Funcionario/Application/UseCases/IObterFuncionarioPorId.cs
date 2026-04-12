namespace GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public interface IObterFuncionarioPorId
{
    Task<ApiResponse<ResponseFuncionarioDTO>> Execute(Guid id);
}