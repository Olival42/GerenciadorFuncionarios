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

        var departamento = await _context.Departamento.FindAsync(data.DepartamentoId);

        if (departamento == null)
            throw new EntityNotFoundException("Departamento não encontrado.");

        if (!departamento.IsActive)
            throw new InactiveEntityException("Não é possível associar um funcionário a um departamento inativo.");

        var func = data.Adapt<Funcionario>();
        
        func.Departamento = departamento;

        _context.Funcionario.Add(func);
        await _context.SaveChangesAsync();

        var dto = func.Adapt<ResponseFuncionarioDTO>();

        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> ObterFuncionarioPorId(Guid id)
    {
        var func = await _context.Funcionario.FindAsync(id);

        if (func == null)
            throw new EntityNotFoundException("Funcionário não encontrado.");

        if (!func.IsActive)
            throw new InactiveEntityException("Funcionário está inativo");

        var dto = func.Adapt<ResponseFuncionarioDTO>();
        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }

    public async Task InativarPorId(Guid id)
    {
        var func = await _context.Funcionario.FindAsync(id);

        if (func == null)
            throw new EntityNotFoundException("Funcionário não encontrado.");

        if (!func.IsActive)
            throw new InactiveEntityException("Funcionário já está inativo");

        func.IsActive = false;

        await _context.SaveChangesAsync();
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> Atualizar(Guid id, UpdateFuncionarioDTO data)
    {
        var func = await _context.Funcionario.FindAsync(id);

        if (func == null)
            throw new EntityNotFoundException("Funcionário não encontrado.");

        if (!func.IsActive)
            throw new InactiveEntityException("Funcionário está inativo");

        if(data.Name is not null)
            func.Name = data.Name;

        if(data.Phone is not null)
            func.Phone = data.Phone;

        if(data.Email is not null)
            func.Email = data.Email;

        await _context.SaveChangesAsync();

        var dto = func.Adapt<ResponseFuncionarioDTO>();
        return ApiResponse<ResponseFuncionarioDTO>.Ok(dto);
    }

    public async Task<ApiResponse<ResponseFuncionarioDTO>> AtualizarDepartamento(Guid id, UpdateDepartamentoFuncionario data)
    {
        var func = await _context.Funcionario.FindAsync(id);

        if (func == null)
            throw new EntityNotFoundException("Funcionário não encontrado.");

        if (!func.IsActive)
            throw new InactiveEntityException("Funcionário está inativo");

        var departamento = await _context.Departamento.FindAsync(data.DepartamentoId);

        if (departamento == null)
            throw new EntityNotFoundException("Departamento não encontrado.");

        if (!departamento.IsActive)
            throw new InactiveEntityException("Não é possível associar um funcionário a um departamento inativo.");

        func.Departamento = departamento;

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
            .Select(d => d.Adapt<ResponseFuncionarioDTO>())
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
