namespace GerenciadorFuncionarios.Mappings.Departamento;

using System;
using Mapster;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.DTOs.Departamento.Requests;
using GerenciadorFuncionarios.DTOs.Departamento.Response;

public static class DepartamentoMapping
{
	public static void Register()
	{
        TypeAdapterConfig<RegistrarDepartamentoDTO, Departamento>
        .NewConfig()
        .Map(dest => dest.Name, src => src.Name);

        TypeAdapterConfig<Departamento, ResponseDepartamentoDTO>
        .NewConfig()
        .Map(dest => dest.Id, src => src.Id)
        .Map(dest => dest.Name, src => src.Name)
        .Map(dest => dest.IsActive, src => src.IsActive);
    }
}
