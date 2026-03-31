namespace GerenciadorFuncionarios.Adapters;

using GerenciadorFuncionarios.DTOs.Departamento.Response;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Shared.Responses;

public interface IDepartamentoRepository
{
    Task Add(Departamento departamento);
    Task<Departamento?> GetById(Guid id);
    Task<bool> AnyByNameAsync(string name);
    Task SaveChangesAsync();
    Task<PaginationResponse<ResponseDepartamentoDTO>> GetAllAsync(
        int page, int pageSize, Guid? departamentoId = null);
}