namespace GerenciadorFuncionarios.Modules.Produto.Application.Services;

using GerenciadorFuncionarios.Modules.Produto.Domain.Models;

public interface IEstoqueService
{
    Task<Produto> BaixarEstoque(Guid produtoId, int quantidade);
    Task<Produto> EntradaEstoque(Guid produtoId, int quantidade);
    Task VerificarEstoqueBaixoAsync(int limite = 5);
}