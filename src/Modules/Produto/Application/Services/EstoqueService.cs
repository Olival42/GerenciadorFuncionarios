namespace GerenciadorFuncionarios.Modules.Produto.Application.Services;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using GerenciadorFuncionarios.Modules.Produto.Web.Hubs;

public class EstoqueService : IEstoqueService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IHubContext<EstoqueHub> _hubContext;

    public EstoqueService(
        IProdutoRepository produtoRepository,
        IHubContext<EstoqueHub> hubContext)
    {
        _produtoRepository = produtoRepository;
       _hubContext = hubContext;
    }

    public async Task<Produto> BaixarEstoque(Guid produtoId, int quantidade)
    {
        throw new NotImplementedException();
    }
    public async Task<Produto> EntradaEstoque(Guid produtoId, int quantidade)
    {
        throw new NotImplementedException();
    }

        public async Task VerificarEstoqueBaixoAsync(int limite = 5)
    {
        throw new NotImplementedException();
    }
}