namespace GerenciadorFuncionarios.Ports;

using GerenciadorFuncionarios.Adapters;
using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.DTOs.Departamento.Response;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using Mapster;

public class DepartamentoRepository : IDepartamentoRepository
{
    private readonly AppDbContext _context;

    public DepartamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Add(Departamento departamento)
    {
        _context.Departamento.Add(departamento);
        await _context.SaveChangesAsync();
    }

    public async Task<Departamento?> GetById(Guid id)
    {
        return await _context.Departamento
                    .Where(f => f.Id == id && f.IsActive)
                    .FirstOrDefaultAsync();
    }

    public async Task<bool> AnyByNameAsync(string name)
    {
        return await _context.Departamento.AnyAsync(d => d.Name == name);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<PaginationResponse<ResponseDepartamentoDTO>> GetAllAsync(
        int page, int pageSize, Guid? departamentoId = null)
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
            .ProjectToType<ResponseDepartamentoDTO>()
            .ToListAsync();

        return new PaginationResponse<ResponseDepartamentoDTO>(
            Items: items,
            Page: page,
            PageSize: pageSize,
            TotalItems: totalItems
        );
    }
}