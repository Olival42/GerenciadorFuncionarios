using System;

using Mapster;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.DTOs.Departamento.Requests;
using GerenciadorFuncionarios.DTOs.Departamento.Response;

public static class MapsterConfig
{

    public static void RegisterMappings()
	{
		TypeAdapterConfig<RegistrarDepartamentoDTO, Departamento>
        .NewConfig()
        .Map(dest => dest.Name, src => src.Name);

    }
}