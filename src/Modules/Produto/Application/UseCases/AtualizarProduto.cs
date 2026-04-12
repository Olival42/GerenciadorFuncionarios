namespace GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public class AtualizarProduto : IAtualizarProduto
{
    private readonly IProdutoRepository _repository;

    public AtualizarProduto(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<ResponseProdutoDTO>> Execute(Guid id, UpdateProdutoDTO data)
    {
        throw new NotImplementedException();
    }
}
