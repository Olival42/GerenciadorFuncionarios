namespace GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Produto.Application.Services;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public class EntradaEstoque : IEntradaEstoque
{
    private readonly IEstoqueService _estoqueService;

    public EntradaEstoque(IEstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    public async Task<ApiResponse<ResponseProdutoDTO>> Execute(Guid id, UpdateEstoqueDTO data)
    {
        throw new NotImplementedException();
    }
}