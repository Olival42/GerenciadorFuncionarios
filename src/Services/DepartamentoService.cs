namespace GerenciadorFuncionarios.Services;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.DTOs.Departamento.Requests;
using GerenciadorFuncionarios.DTOs.Departamento.Response;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Shared.Responses;
using Mapster;
using GerenciadorFuncionarios.Exceptions;

public class DepartamentoService
{
    private readonly AppDbContext _context;

    private readonly ILogger<DepartamentoService> _logger;

    public DepartamentoService(AppDbContext context, ILogger<DepartamentoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<ResponseDepartamentoDTO>> RegistrarDepartamentoAsync(RegistrarDepartamentoDTO data)
    {
        _logger.LogInformation(
            "Tentativa de cadastro de departamento. Nome: {Name}",
            data.Name);

        if (await _context.Departamento.AnyAsync(d => d.Name == data.Name))
        {
            _logger.LogWarning(
                "Departamento já cadastrado. Nome: {Name}",
                data.Name);

            throw new EntityAlreadyExistsException($"Departamento {data.Name} já exite.");
        }

        var departamento = data.Adapt<Departamento>();

        _context.Departamento.Add(departamento);
        await _context.SaveChangesAsync();

        var dto = departamento.Adapt<ResponseDepartamentoDTO>();

        _logger.LogInformation(
            "Departamento cadastrado. Id: {Id} Nome: {Name}",
            departamento.Id,
            departamento.Name);

        return ApiResponse<ResponseDepartamentoDTO>.Ok(dto);
    }

    public async Task<ApiResponse<ResponseDepartamentoDTO>> ObterDepartamentoPorId(Guid id)
    {
        var departamento = await _context.Departamento
            .Where(d => d.Id == id && d.IsActive)
            .ProjectToType<ResponseDepartamentoDTO>()
            .FirstOrDefaultAsync();

        if (departamento == null)
        {
            _logger.LogWarning(
                "Departamento não encontrado. Id: {Id}",
                id);

            throw new EntityNotFoundException("Departamento não encontrado.");
        }

        return ApiResponse<ResponseDepartamentoDTO>.Ok(departamento);
    }

    public async Task InativarDepartamento(Guid id)
    {
        _logger.LogInformation(
            "Tentativa de inativação de departamento. Id: {Id}",
            id);

        var departamento = await _context.Departamento
            .Where(d => d.Id == id && d.IsActive)
            .FirstOrDefaultAsync();

        if (departamento == null)
        {
            _logger.LogWarning(
                "Departamento não encontrado. Id: {Id}",
                id);

            throw new EntityNotFoundException("Departamento não encontrado.");
        }

        departamento.IsActive = false;

        _context.Departamento.Update(departamento);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Departamento inativado. Id: {Id} Nome: {Name}",
            departamento.Id,
            departamento.Name);
    }

    public async Task<ApiResponse<PaginationResponse<ResponseDepartamentoDTO>>> ObterTodosDepartamentos(int page, int pageSize)
    {
        var totalItems = await _context.Departamento.CountAsync();

        var items = await _context.Departamento
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => d.Adapt<ResponseDepartamentoDTO>())
            .ToListAsync();

        var paginated = new PaginationResponse<ResponseDepartamentoDTO>
        (
            Items: items,
            Page: page,
            TotalItems: totalItems,
            PageSize: pageSize
        );

        return ApiResponse<PaginationResponse<ResponseDepartamentoDTO>>.Ok(paginated);
    }
}
