using System;
using GerenciadorFuncionarios.Domain.ValueObjects;
using Xunit;

public class CpfTests
{
    [Fact]
    public void Cpf_Should_Construct_With_Valid_CPF()
    {
        var cpf = new Cpf("88917605010");

        Assert.Equal("88917605010", cpf.Value);
        Assert.Equal("88917605010", (string)cpf);
    }

    [Fact]
    public void Cpf_Should_Construct_With_Formatted_CPF_And_Normalize()
    {
        var cpf = new Cpf("889.176.050-10");

        Assert.Equal("88917605010", cpf.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Cpf_Should_Throw_When_Empty_Or_Null(string value)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Cpf(value!));
        Assert.Equal("CPF não pode ser vazio.", exception.Message);
    }

    [Theory]
    [InlineData("12345678901")]
    [InlineData("11111111111")]
    [InlineData("88927605010")]
    public void Cpf_Should_Throw_When_Invalid(string value)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Cpf(value));
        Assert.Equal("CPF inválido.", exception.Message);
    }
}
