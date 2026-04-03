using Xunit;
using Mapster;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;

public class FuncionarioMappingTests
{
    [Fact]
    public void RegisterFuncionarioDTO_Should_Map_To_Funcionario_Correctly()
    {
        var dto = new RegisterFuncionarioDTO
        {
            Name = "João da Silva",
            UserName = "teste",
            Password = "Senha@123",
            Role = Role.GERENTE,
            Phone = "44999999999",
            CPF = "52998224725",
        };

        var funcionario = dto.Adapt<Funcionario>();

        Assert.Equal(dto.Name, funcionario.Name);
        Assert.Equal(dto.UserName, funcionario.UserName);
        Assert.Equal(dto.Phone, funcionario.Phone);
        Assert.Equal(dto.CPF, funcionario.CPF);
        Assert.Equal(dto.Role, funcionario.Role);
    }

    [Fact]
    public void Funcionario_Should_Map_To_ResponseFuncionarioDTO_Correctly()
    {
        var func = new Funcionario
        {
            Id = Guid.NewGuid(),
            Name = "Maria",
            UserName = "teste",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Phone = null!,
            CPF = "98765432100",
            Role = Role.FUNCIONARIO,
            IsActive = true
        };

        var dto = func.Adapt<ResponseFuncionarioDTO>();

        Assert.Equal(func.Id, dto.Id);
        Assert.Equal(func.Name, dto.Name);
        Assert.Equal(func.UserName, dto.UserName);
        Assert.Equal(func.Phone, dto.Phone);
        Assert.Equal(func.CPF, dto.CPF);
        Assert.Equal(func.Role, dto.Role);
        Assert.Equal(func.IsActive, dto.IsActive);
    }
}