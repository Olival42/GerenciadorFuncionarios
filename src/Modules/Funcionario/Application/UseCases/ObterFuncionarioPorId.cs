namespace GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;


public class ObterFuncionarioPorId : IObterFuncionarioPorId
{
    private readonly IFuncionarioRepository _repository;

    public ObterFuncionarioPorId(IFuncionarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> Execute(Guid id)
    {
        throw new NotImplementedException();
    }
}
