using System;

using Mapster;
using GerenciadorFuncionarios.Mappings.Departamento;


public static class MapsterConfig
{

    public static void RegisterMappings()
	{
        DepartamentoMapping.Register();
    }
}