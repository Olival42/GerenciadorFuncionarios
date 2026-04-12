namespace GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;


public class ObterFuncionarioLogado : IObterFuncionarioLogado
{
    private readonly IFuncionarioRepository _repository;
    private readonly IUserContextService _userContext;

    public ObterFuncionarioLogado(IFuncionarioRepository repository, IUserContextService userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> Execute()
    {
        throw new NotImplementedException();
    }
}
