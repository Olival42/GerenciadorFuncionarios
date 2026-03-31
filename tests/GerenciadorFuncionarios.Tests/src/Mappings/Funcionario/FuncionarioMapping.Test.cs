using Xunit;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Enums;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using Mapster;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;

public class FuncionarioMappingTests
{
    [Fact]
    public void RegisterFuncionarioDTO_Should_Map_To_Funcionario_Correctly()
    {
        var dto = new RegisterFuncionarioDTO
        {
            Name = "João da Silva",
            Email = "joao@email.com",
            Password = "Senha@123",
            Role = Role.ADMIN,
            Phone = "44999999999",
            CPF = "52998224725",
            DepartamentoId = Guid.NewGuid()
        };

        var funcionario = dto.Adapt<Funcionario>();

        Assert.Equal(dto.Name, funcionario.Name);
        Assert.Equal(dto.Email, funcionario.Email);
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
            Email = "maria@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Phone = null!,
            CPF = "98765432100",
            DepartamentoId = Guid.NewGuid(),
            Role = Role.FUNCIONARIO,
            IsActive = true
        };

        var dto = func.Adapt<ResponseFuncionarioDTO>();

        Assert.Equal(func.Id, dto.Id);
        Assert.Equal(func.Name, dto.Name);
        Assert.Equal(func.Email, dto.Email);
        Assert.Equal(func.Phone, dto.Phone);
        Assert.Equal(func.CPF, dto.CPF);
        Assert.Equal(func.DepartamentoId, dto.DepartamentoId);
        Assert.Equal(func.Role, dto.Role);
        Assert.Equal(func.IsActive, dto.IsActive);
    }
}