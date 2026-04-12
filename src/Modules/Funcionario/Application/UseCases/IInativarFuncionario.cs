namespace GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

public interface IInativarFuncionario
{
    Task Execute(Guid id);
}