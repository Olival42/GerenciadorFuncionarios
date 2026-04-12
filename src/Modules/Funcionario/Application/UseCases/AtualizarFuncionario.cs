namespace GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

using System;
using System.Threading.Tasks;
using BCrypt.Net;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;


public class AtualizarFuncionario : IAtualizarFuncionario
{
    private readonly IFuncionarioRepository _repository;

    public AtualizarFuncionario(IFuncionarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> Execute(Guid id, UpdateFuncionarioDTO data)
    {
        throw new NotImplementedException();
    }
}
