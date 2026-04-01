namespace GerenciadorFuncionarios.Modules.Produto.Infrastructure.Services;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Hubs;
using GerenciadorFuncionarios.Modules.Produto.Application.Services;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IHubContext<EstoqueHub> _hubContext;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(
        IProdutoRepository produtoRepository,
        IHubContext<EstoqueHub> hubContext,
        ILogger<ProdutoService> logger)
    {
        _produtoRepository = produtoRepository;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<ApiResponse<ResponseProdutoDTO>> RegistrarProdutoAsync(RegisterProdutoDTO data)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse<ResponseProdutoDTO>> ObterProdutoPorId(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task InativarPorId(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse<ResponseProdutoDTO>> Atualizar(Guid id, UpdateProdutoDTO data)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse<ResponseProdutoDTO>> EntradaEstoque(Guid id, int quantidade)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse<ResponseProdutoDTO>> BaixarEstoque(Guid id, int quantidade)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse<PaginationResponse<ResponseProdutoDTO>>> ObterTodosFuncionarios(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task VerificarEstoqueBaixoAsync(int limite = 5)
    {
        throw new NotImplementedException();
    }
}