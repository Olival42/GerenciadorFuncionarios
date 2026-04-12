namespace GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public interface IObterFuncionarioLogado
{
    Task<ApiResponse<ResponseFuncionarioDTO>> Execute();
}