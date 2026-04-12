using System.Threading.Tasks;

namespace GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

public interface IInativarProduto
{
    Task Execute(Guid id);
}