namespace GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;

using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;

public interface IFuncionarioRepository
{
    Task<Funcionario?> GetByUserName(string userName);
    Task<Funcionario?> GetByIdAsync(Guid id);
    Task Add(Funcionario funcionario);
    Task<bool> AnyByCPFAsync(string cpf);
    Task<bool> AnyByUserNameAsync(string userName);
    Task SaveChangesAsync();
    Task<PaginationResponse<ResponseFuncionarioDTO>> GetAllAsync(
        int page, int pageSize);
}