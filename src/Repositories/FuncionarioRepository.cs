namespace GerenciadorFuncionarios.Repositories;

using GerenciadorFuncionarios.Adapters;
using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Shared.Responses;

public class FuncionarioRepository : IFuncionarioRepository
{
    private readonly AppDbContext _context;

    public FuncionarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Funcionario?> GetByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<Funcionario?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task Add(Funcionario funcionario)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AnyByCPFAsync(string cpf)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AnyByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<PaginationResponse<ResponseFuncionarioDTO>> GetAllAsync(
        int page,
        int pageSize)
    {
        throw new NotImplementedException();
    }
}