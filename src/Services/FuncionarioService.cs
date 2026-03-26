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

    public FuncionarioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> RegistrarFuncionarioAsync(RegisterFuncionarioDTO data)
    {
        if (await _context.Funcionario.AnyAsync(f => f.CPF == data.CPF))
            throw new CPFAlreadyExistsException("CPF já cadastrado para outro funcionário.");

        if (await _context.Funcionario.AnyAsync(f => f.Email == data.Email))
            throw new EmailAlreadyExistsException("Email já cadastrado para outro funcionário.");

        var departamento = await _context.Departamento
            .Where(d => d.Id == data.DepartamentoId && d.IsActive)
            .FirstOrDefaultAsync();

        if (departamento == null)
            throw new EntityNotFoundException("Departamento não encontrado.");

        var func = data.Adapt<Funcionario>();
        
        func.PasswordHash = BCrypt.HashPassword(data.Password);
        func.DepartamentoId = departamento.Id;

        _context.Funcionario.Add(func);
        await _context.SaveChangesAsync();

        var dto = func.Adapt<ResponseFuncionarioDTO>();

        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> ObterFuncionarioPorId(Guid id)
    {
        var func = await _context.Funcionario
            .Where(f => f.Id == id && f.IsActive)
            .ProjectToType<ResponseFuncionarioDTO>()
            .FirstOrDefaultAsync();

        if (func == null)
            throw new EntityNotFoundException("Funcionário não encontrado.");

        return ApiResponse<ResponseFuncionarioDTO>.Ok(func);
    }

    public async Task InativarPorId(Guid id)
    {
        var func = await _context.Funcionario
            .Where(f => f.Id == id && f.IsActive)
            .FirstOrDefaultAsync();

        if (func == null)
            throw new EntityNotFoundException("Funcionário não encontrado.");

        func.IsActive = false;

        await _context.SaveChangesAsync();
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> Atualizar(Guid id, UpdateFuncionarioDTO data)
    {
        var func = await _context.Funcionario
            .Where(f => f.Id == id && f.IsActive)
            .FirstOrDefaultAsync();

        if (func == null)
            throw new EntityNotFoundException("Funcionário não encontrado.");

        if (data.Name is not null)
            func.Name = data.Name;

        if (data.Phone is not null)
            func.Phone = data.Phone;

        await _context.SaveChangesAsync();

        var dto = func.Adapt<ResponseFuncionarioDTO>();
        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> AtualizarDepartamento(Guid id, UpdateDepartamentoFuncionario data)
    {
        var func = await _context.Funcionario
            .Where(f => f.Id == id && f.IsActive)
            .FirstOrDefaultAsync();

        if (func == null)
            throw new EntityNotFoundException("Funcionário não encontrado.");

        var departamento = await _context.Departamento
            .Where(d => d.Id == data.DepartamentoId && d.IsActive)
            .FirstOrDefaultAsync();

        if (departamento == null)
            throw new EntityNotFoundException("Departamento não encontrado.");

        func.DepartamentoId = departamento.Id;

        await _context.SaveChangesAsync();

        var dto = func.Adapt<ResponseFuncionarioDTO>();
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
