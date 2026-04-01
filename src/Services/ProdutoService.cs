namespace GerenciadorFuncionarios.Services;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.AspNetCore.SignalR;
using GerenciadorFuncionarios.Hubs;
using GerenciadorFuncionarios.Adapters;

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
        // TODO: Mapear DTO -> Entidade
        // TODO: Persistir no repositório
        // TODO: Mapear entidade -> DTO

        throw new NotImplementedException();
    }

    public async Task<ApiResponse<ResponseProdutoDTO>> ObterProdutoPorId(Guid id)
    {
        // TODO: Mapear para DTO
        throw new NotImplementedException();
    }

    public async Task InativarPorId(Guid id)
    {
        // TODO: Marcar como inativo
        // TODO: Persistir alteração

        throw new NotImplementedException();
    }

    public async Task<ApiResponse<ResponseProdutoDTO>> Atualizar(Guid id, UpdateProdutoDTO data)
    {
        // TODO: Atualizar campos permitidos
        // TODO: Persistir
        // TODO: Mapear DTO

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
        // TODO: Buscar paginado
        // TODO: Mapear DTO

        throw new NotImplementedException();
    }

    public async Task VerificarEstoqueBaixoAsync(int limite = 5)
    {
        // TODO: Buscar produtos ativos
        // TODO: Filtrar estoque baixo
        // TODO: Enviar alerta via SignalR

        throw new NotImplementedException();
    }
}