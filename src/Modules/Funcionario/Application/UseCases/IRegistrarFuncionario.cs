namespace GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public interface IRegistrarFuncionario
{
    Task<ApiResponse<ResponseFuncionarioDTO>> Execute(RegisterFuncionarioDTO data);
}