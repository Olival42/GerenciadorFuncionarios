namespace GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public interface IAtualizarFuncionario
{
    Task<ApiResponse<ResponseFuncionarioDTO>> Execute(Guid id, UpdateFuncionarioDTO data);
}