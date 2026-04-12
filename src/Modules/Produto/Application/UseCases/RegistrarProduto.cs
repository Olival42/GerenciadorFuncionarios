namespace GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

public class RegistrarProduto : IRegistrarProduto
{
    private readonly IProdutoRepository _repository;

    public RegistrarProduto(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<ResponseProdutoDTO>> Execute(RegisterProdutoDTO data)
    {
        throw new NotImplementedException();
    }
}
