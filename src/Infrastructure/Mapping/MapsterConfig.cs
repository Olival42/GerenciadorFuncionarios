using GerenciadorFuncionarios.Modules.Funcionario.Domain.Mappings;
using GerenciadorFuncionarios.Modules.Produto.Domain.Mappings;

namespace GerenciadorFuncionarios.Infrastructure.Mappings;

public static class MapsterConfig
{

    public static void RegisterMappings()
	{
        FuncionarioMapping.Register();
        ProdutoMapping.Register();
    }
}