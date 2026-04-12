using System.Threading.Tasks;

namespace GerenciadorFuncionarios.Modules.Auth.Application.UseCases;

public interface ILogout
{
    Task Execute(string refreshToken);
}
