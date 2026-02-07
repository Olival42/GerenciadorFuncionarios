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

	public DepartamentoService(AppDbContext context)
	{
		_context = context;
	}

	public async Task<ApiResponse<ResponseDepartamentoDTO>> RegistrarDepartamentoAsync(RegistrarDepartamentoDTO data)
	{
		if(await _context.Departamento.AnyAsync(d => d.Name == data.Name))
		{
			throw new EntityAlreadyExistsException($"Departamento {data.Name} já exite.");
		}

		var departamento = data.Adapt<Departamento>();

		_context.Departamento.Add(departamento);
		await _context.SaveChangesAsync();

		var dto = departamento.Adapt<ResponseDepartamentoDTO>();

		return ApiResponse<ResponseDepartamentoDTO>.Ok(dto);
    }

	public async Task<ApiResponse<ResponseDepartamentoDTO>> ObterDepartamentoPorId(Guid id)
	{
		var departamento = await _context.Departamento.FindAsync(id);

        if (departamento == null)
            throw new EntityNotFoundException("Departamento não encontrado.");

        var dto = departamento.Adapt<ResponseDepartamentoDTO>();
        return ApiResponse<ResponseDepartamentoDTO>.Ok(dto);
    }

    public async Task DeletarDepartamento(Guid id)
    {
        var departamento = await _context.Departamento.FindAsync(id);

        if (departamento == null)
            throw new EntityNotFoundException("Departamento não encontrado.");
		
		_context.Departamento.Remove(departamento);
		await _context.SaveChangesAsync();
    }

    public async Task<ApiResponse<PaginationResponse<ResponseDepartamentoDTO>>> ObterTodosDepartamentos(int page, int pageSize)
    {
        var totalItems = await _context.Departamento.CountAsync();

        var items = await _context.Departamento
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
