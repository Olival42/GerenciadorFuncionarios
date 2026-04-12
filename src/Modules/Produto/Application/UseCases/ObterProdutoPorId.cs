namespace GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

public class ObterProdutoPorId : IObterProdutoPorId
{
    private readonly IProdutoRepository _repository;

    public ObterProdutoPorId(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<ResponseProdutoDTO>> Execute(Guid id)
    {
        throw new NotImplementedException();
    }
}
