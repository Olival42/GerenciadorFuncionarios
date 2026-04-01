using GerenciadorFuncionarios.DTOs.Funcionario.Responses;

namespace GerenciadorFuncionarios.Adapters;

public interface IEstoqueHub
{
    Task EnviarAlertaProduto(ProdutoAlertaDTO produto);
}