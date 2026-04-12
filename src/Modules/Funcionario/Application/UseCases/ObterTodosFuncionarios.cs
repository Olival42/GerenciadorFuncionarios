namespace GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public class ObterTodosFuncionarios : IObterTodosFuncionarios
{
    private readonly IFuncionarioRepository _repository;

    public ObterTodosFuncionarios(IFuncionarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>> Execute(int page, int pageSize)
    {
        throw new NotImplementedException();
    }
}
