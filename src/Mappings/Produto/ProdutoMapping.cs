namespace GerenciadorFuncionarios.Mappings.Funcionario;

using Mapster;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;

public static class ProdutoMapping
{
	public static void Register()
	{
        TypeAdapterConfig<RegisterProdutoDTO, Produto>
            .NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.Price, src => src.Price);

        TypeAdapterConfig<Produto, ResponseProdutoDTO>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.Price, src => src.Price)
            .Map(dest => dest.IsActive, src => src.IsActive);
    }
}