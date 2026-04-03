namespace GerenciadorFuncionarios.Modules.Funcionario.Infrastructure.Repositories;

using GerenciadorFuncionarios.Infrastructure;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public class FuncionarioRepository : IFuncionarioRepository
{
    private readonly AppDbContext _context;

    public FuncionarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Funcionario?> GetByUserName(string userName)
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

    public Task<bool> AnyByUserNameAsync(string userName)
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