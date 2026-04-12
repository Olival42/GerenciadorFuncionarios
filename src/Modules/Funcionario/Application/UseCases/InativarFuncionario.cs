namespace GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;

public class InativarFuncionario : IInativarFuncionario
{
    private readonly IFuncionarioRepository _repository;

    public InativarFuncionario(IFuncionarioRepository repository)
    {
        _repository = repository;
    }

    public async Task Execute(Guid id)
    {
        throw new NotImplementedException();
    }
}
