namespace GerenciadorFuncionarios.Mappings;

using GerenciadorFuncionarios.Mappings.Departamento;
using GerenciadorFuncionarios.Mappings.Funcionario;


public static class MapsterConfig
{

    public static void RegisterMappings()
	{
        DepartamentoMapping.Register();
        FuncionarioMapping.Register();
    }
}