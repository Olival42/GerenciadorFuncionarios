namespace GerenciadorFuncionarios.Mappings.Funcionario;

using System;
using Mapster;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;

public static class FuncionarioMapping
{
	public static void Register()
	{
        TypeAdapterConfig<RegisterFuncionarioDTO, Funcionario>
            .NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.CPF, src => src.CPF);

        TypeAdapterConfig<Funcionario, ResponseFuncionarioDTO>
            .NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Phone, src => src.Phone)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.CPF, src => src.CPF)
            .Map(dest => dest.DepartamentoId, src => src.DepartamentoId);
    }
}
