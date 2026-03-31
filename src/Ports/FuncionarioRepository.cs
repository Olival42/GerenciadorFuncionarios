namespace GerenciadorFuncionarios.Ports;

using GerenciadorFuncionarios.Adapters;
using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using Mapster;

public class FuncionarioRepository : IFuncionarioRepository
{
    private readonly AppDbContext _context;

    public FuncionarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Funcionario?> GetByEmail(string email)
    {
        return await _context.Funcionario
                .Where(f => f.Email == email && f.IsActive)
                .FirstOrDefaultAsync();
    }

    public async Task<Funcionario?> GetByIdAsync(Guid id)
    {
        return await _context.Funcionario
                    .Where(f => f.Id == id && f.IsActive)
                    .FirstOrDefaultAsync();
    }

    public async Task Add(Funcionario funcionario)
    {
        _context.Funcionario.Add(funcionario);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AnyByCPFAsync(string cpf)
    {
        return await _context.Funcionario.AnyAsync(f => f.CPF == cpf);
    }

    public async Task<bool> AnyByEmailAsync(string email)
    {
        return await _context.Funcionario.AnyAsync(f => f.Email == email);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<PaginationResponse<ResponseFuncionarioDTO>> GetAllAsync(
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
            .ProjectToType<ResponseFuncionarioDTO>()
            .ToListAsync();

        return new PaginationResponse<ResponseFuncionarioDTO>(
            Items: items,
            Page: page,
            PageSize: pageSize,
            TotalItems: totalItems
        );
    }
}