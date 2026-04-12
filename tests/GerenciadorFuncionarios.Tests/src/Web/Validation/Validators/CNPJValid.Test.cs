using Xunit;
using GerenciadorFuncionarios.Web.Validation.Validators;

namespace GerenciadorFuncionarios.Tests.Validation;

public class CNPJCalidTests
{
    [Theory]
    [InlineData("11222333000181")]
    [InlineData("11.222.333/0001-81")]
    [InlineData("45.723.174/0001-10")]
    public void IsValid_Should_Return_True_When_CNPJ_Is_Valid(string cnpj)
    {
        var result = CNPJCalid.IsValid(cnpj);

        Assert.True(result);
    }

    [Theory]
    [InlineData("11222333000182")]
    [InlineData("11.222.333/0001-82")]
    [InlineData("12345678000100")]
    public void IsValid_Should_Return_False_When_CNPJ_Is_Invalid(string cnpj)
    {
        var result = CNPJCalid.IsValid(cnpj);

        Assert.False(result);
    }

    [Theory]
    [InlineData("11111111111111")]
    [InlineData("00000000000000")]
    [InlineData("22222222222222")]
    public void IsValid_Should_Return_False_When_All_Digits_Are_Equal(string cnpj)
    {
        var result = CNPJCalid.IsValid(cnpj);

        Assert.False(result);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789")]
    [InlineData("123456789012345")]
    public void IsValid_Should_Return_False_When_Length_Is_Invalid(string cnpj)
    {
        var result = CNPJCalid.IsValid(cnpj);

        Assert.False(result);
    }

    [Fact]
    public void IsValid_Should_Return_False_When_Null()
    {
        var result = CNPJCalid.IsValid(null!);

        Assert.False(result);
    }

    [Fact]
    public void IsValid_Should_Return_False_When_Empty()
    {
        var result = CNPJCalid.IsValid("");

        Assert.False(result);
    }
}