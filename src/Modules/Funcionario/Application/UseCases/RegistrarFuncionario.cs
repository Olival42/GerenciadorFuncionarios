namespace GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;


public class RegistrarFuncionario : IRegistrarFuncionario
{
    private readonly IFuncionarioRepository _repository;

    public RegistrarFuncionario(IFuncionarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> Execute(RegisterFuncionarioDTO data)
    {
        throw new NotImplementedException();
    }
}