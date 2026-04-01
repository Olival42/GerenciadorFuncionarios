namespace GerenciadorFuncionarios.Adapters;

using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Shared.Responses;

public interface IFuncionarioRepository
{
    Task<Funcionario?> GetByEmail(string email);
    Task<Funcionario?> GetByIdAsync(Guid id);
    Task Add(Funcionario funcionario);
    Task<bool> AnyByCPFAsync(string cpf);
    Task<bool> AnyByEmailAsync(string email);
    Task SaveChangesAsync();
    Task<PaginationResponse<ResponseFuncionarioDTO>> GetAllAsync(
        int page, int pageSize);
}