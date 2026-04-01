namespace GerenciadorFuncionarios.Mappings;

using GerenciadorFuncionarios.Mappings.Funcionario;


public static class MapsterConfig
{

    public static void RegisterMappings()
	{
        FuncionarioMapping.Register();
        ProdutoMapping.Register();
    }
}