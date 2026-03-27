namespace GerenciadorFuncionarios.Services;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Shared.Responses;
using Mapster;
using GerenciadorFuncionarios.Exceptions;
using BCrypt.Net;

public class FuncionarioService
{
    private readonly AppDbContext _context;

    private readonly ILogger<FuncionarioService> _logger;

    public FuncionarioService(AppDbContext context, ILogger<FuncionarioService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> RegistrarFuncionarioAsync(RegisterFuncionarioDTO data)
    {
        _logger.LogInformation(
            "Tentativa de cadastro de funcionário. Email: {Email}",
            data.Email);

        if (await _context.Funcionario.AnyAsync(f => f.CPF == data.CPF))
        {
            _logger.LogWarning(
                "CPF já cadastrado. CPF: {CPF}",
                data.CPF);

            throw new CPFAlreadyExistsException("CPF já cadastrado para outro funcionário.");
        }

        if (await _context.Funcionario.AnyAsync(f => f.Email == data.Email))
        {
            _logger.LogWarning(
                "Email já cadastrado. Email: {Email}",
                data.Email);

            throw new EmailAlreadyExistsException("Email já cadastrado para outro funcionário.");
        }

        var departamento = await _context.Departamento
            .Where(d => d.Id == data.DepartamentoId && d.IsActive)
            .FirstOrDefaultAsync();

        if (departamento == null)
        {
            _logger.LogWarning(
                "Departamento não encontrado. Id: {Id}",
                data.DepartamentoId);

            throw new EntityNotFoundException("Departamento não encontrado.");
        }

        var func = data.Adapt<Funcionario>();

        func.PasswordHash = BCrypt.HashPassword(data.Password);
        func.DepartamentoId = departamento.Id;

        _context.Funcionario.Add(func);
        await _context.SaveChangesAsync();

        var dto = func.Adapt<ResponseFuncionarioDTO>();

        _logger.LogInformation(
            "Funcionário cadastrado. Id: {Id} Email: {Email}",
            func.Id,
            func.Email);

        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> ObterFuncionarioPorId(Guid id)
    {
        var func = await _context.Funcionario
            .Where(f => f.Id == id && f.IsActive)
            .ProjectToType<ResponseFuncionarioDTO>()
            .FirstOrDefaultAsync();

        if (func == null)
        {
            _logger.LogWarning(
                "Funcionário não encontrado. Id: {Id}",
                id);

            throw new EntityNotFoundException("Funcionário não encontrado.");
        }

        return ApiResponse<ResponseFuncionarioDTO>.Ok(func);
    }

    public async Task InativarPorId(Guid id)
    {
        _logger.LogInformation(
            "Tentativa de inativação de funcionário. Id: {Id}",
            id);

        var func = await _context.Funcionario
            .Where(f => f.Id == id && f.IsActive)
            .FirstOrDefaultAsync();

        if (func == null)
        {
            _logger.LogWarning(
                "Funcionário não encontrado. Id: {Id}",
                id);

            throw new EntityNotFoundException("Funcionário não encontrado.");
        }

        func.IsActive = false;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Funcionário inativado. Id: {Id} Email: {Email}",
            func.Id,
            func.Email);
    }

    // atualizar função
    public async Task<ApiResponse<ResponseFuncionarioDTO>> Atualizar(Guid id, UpdateFuncionarioDTO data)
    {
        _logger.LogInformation(
            "Tentativa de atualização de funcionário. Id: {Id}",
            id);

        var func = await _context.Funcionario
            .Where(f => f.Id == id && f.IsActive)
            .FirstOrDefaultAsync();

        if (func == null)
        {
            _logger.LogWarning(
                "Funcionário não encontrado. Id: {Id}",
                id);

            throw new EntityNotFoundException("Funcionário não encontrado.");
        }

        if (data.Name is not null)
            func.Name = data.Name;

        if (data.Phone is not null)
            func.Phone = data.Phone;

        await _context.SaveChangesAsync();

        var dto = func.Adapt<ResponseFuncionarioDTO>();

        _logger.LogInformation(
            "Funcionário atualizado. Id: {Id} Email: {Email}",
            func.Id,
            func.Email);

        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> AtualizarDepartamento(Guid id, UpdateDepartamentoFuncionario data)
    {
        _logger.LogInformation(
            "Tentativa de atualização de departamento do funcionário. Id: {Id}",
            id);

        var func = await _context.Funcionario
            .Where(f => f.Id == id && f.IsActive)
            .FirstOrDefaultAsync();

        if (func == null)
        {
            _logger.LogWarning(
                "Funcionário não encontrado. Id: {Id}",
                id);

            throw new EntityNotFoundException("Funcionário não encontrado.");
        }

        var departamento = await _context.Departamento
            .Where(d => d.Id == data.DepartamentoId && d.IsActive)
            .FirstOrDefaultAsync();

        if (departamento == null)
        {
            _logger.LogWarning(
                "Departamento não encontrado. Id: {Id}",
                data.DepartamentoId);

            throw new EntityNotFoundException("Departamento não encontrado.");
        }

        func.DepartamentoId = departamento.Id;

        await _context.SaveChangesAsync();

        var dto = func.Adapt<ResponseFuncionarioDTO>();

        _logger.LogInformation(
            "Departamento do funcionário atualizado. Id: {Id} Email: {Email}",
            func.Id,
            func.Email);

        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }

    public async Task<ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>> ObterTodosFuncionarios(int page, int pageSize, Guid? departamentoId)
    {
        var query = _context.Funcionario
            .Where(f => f.IsActive);

        if (departamentoId.HasValue)
        {
            query = query.Where(f => f.DepartamentoId == departamentoId);
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderBy(f => f.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<ResponseFuncionarioDTO>()
            .ToListAsync();

        var paginated = new PaginationResponse<ResponseFuncionarioDTO>
        (
            Items: items,
            Page: page,
            TotalItems: totalItems,
            PageSize: pageSize
        );

        return ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>.Ok(paginated);
    }
}
