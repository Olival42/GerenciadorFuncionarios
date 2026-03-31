namespace GerenciadorFuncionarios.Services;

using System;
using System.Threading.Tasks;
using GerenciadorFuncionarios.DTOs.Departamento.Requests;
using GerenciadorFuncionarios.DTOs.Departamento.Response;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Shared.Responses;
using Mapster;
using GerenciadorFuncionarios.Exceptions;
using GerenciadorFuncionarios.Adapters;

public class DepartamentoService
{
    private readonly IDepartamentoRepository _repository;

    private readonly ILogger<DepartamentoService> _logger;

    public DepartamentoService(IDepartamentoRepository repository, ILogger<DepartamentoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ApiResponse<ResponseDepartamentoDTO>> RegistrarDepartamentoAsync(RegistrarDepartamentoDTO data)
    {
        _logger.LogInformation(
            "Tentativa de cadastro de departamento. Nome: {Name}",
            data.Name);

        if (await _repository.AnyByNameAsync(data.Name))
        {
            _logger.LogWarning(
                "Departamento já cadastrado. Nome: {Name}",
                data.Name);

            throw new EntityAlreadyExistsException($"Departamento {data.Name} já exite.");
        }

        var departamento = data.Adapt<Departamento>();

        await _repository.Add(departamento);

        var dto = departamento.Adapt<ResponseDepartamentoDTO>();

        _logger.LogInformation(
            "Departamento cadastrado. Id: {Id} Nome: {Name}",
            departamento.Id,
            departamento.Name);

        return ApiResponse<ResponseDepartamentoDTO>.Ok(dto);
    }

    public async Task<ApiResponse<ResponseDepartamentoDTO>> ObterDepartamentoPorId(Guid id)
    {
        var departamento = await _repository.GetById(id);

        if (departamento == null)
        {
            _logger.LogWarning(
                "Departamento não encontrado. Id: {Id}",
                id);

            throw new EntityNotFoundException("Departamento não encontrado.");
        }

        return ApiResponse<ResponseDepartamentoDTO>.Ok(departamento.Adapt<ResponseDepartamentoDTO>());
    }

    public async Task InativarDepartamento(Guid id)
    {
        _logger.LogInformation(
            "Tentativa de inativação de departamento. Id: {Id}",
            id);

        var departamento = await _repository.GetById(id);

        if (departamento == null)
        {
            _logger.LogWarning(
                "Departamento não encontrado. Id: {Id}",
                id);

            throw new EntityNotFoundException("Departamento não encontrado.");
        }

        departamento.IsActive = false;

        await _repository.SaveChangesAsync();

        _logger.LogInformation(
            "Departamento inativado. Id: {Id} Nome: {Name}",
            departamento.Id,
            departamento.Name);
    }

    public async Task<ApiResponse<PaginationResponse<ResponseDepartamentoDTO>>> ObterTodosDepartamentos(int page, int pageSize)
    {
        var paginated = await _repository.GetAllAsync(page, pageSize);

        return ApiResponse<PaginationResponse<ResponseDepartamentoDTO>>.Ok(paginated);
    }
}
