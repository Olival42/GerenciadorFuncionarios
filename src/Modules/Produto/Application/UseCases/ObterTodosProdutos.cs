namespace GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;


public class ObterTodosProdutos : IObterTodosProdutos
{
    private readonly IProdutoRepository _repository;

    public ObterTodosProdutos(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<PaginationResponse<ResponseProdutoDTO>>> Execute(
            int page,
            int pageSize,
            string? name,
            decimal? priceMin,
            decimal? priceMax,
            TipoProduto? type)
    {
        throw new NotImplementedException();
    }
}
