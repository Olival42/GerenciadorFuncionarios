namespace GerenciadorFuncionarios.Services;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Adapters;
using Microsoft.Extensions.Logging;

public class FuncionarioService : IFuncionarioService
{
    private readonly IFuncionarioRepository _funcRepository;
    private readonly ILogger<FuncionarioService> _logger;

    public FuncionarioService(
        IFuncionarioRepository funcRepository,
        ILogger<FuncionarioService> logger)
    {
        _funcRepository = funcRepository;
        _logger = logger;
    }

    public Task<ApiResponse<ResponseFuncionarioDTO>> RegistrarFuncionarioAsync(RegisterFuncionarioDTO data)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<ResponseFuncionarioDTO>> ObterFuncionarioPorId(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task InativarPorId(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<ResponseFuncionarioDTO>> Atualizar(Guid id, UpdateFuncionarioDTO data)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>> ObterTodosFuncionarios(
        int page,
        int pageSize)
    {
        throw new NotImplementedException();
    }
}