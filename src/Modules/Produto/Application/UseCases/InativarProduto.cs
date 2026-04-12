namespace GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;

public class InativarProduto : IInativarProduto
{
    private readonly IProdutoRepository _repository;

    public InativarProduto(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task Execute(Guid id)
    {
        throw new NotImplementedException();
    }
}
