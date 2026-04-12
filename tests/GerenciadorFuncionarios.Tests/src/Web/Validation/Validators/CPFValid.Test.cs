using GerenciadorFuncionarios.Web.Validation.Validators;
using Xunit;

namespace GerenciadorFuncionarios.Tests.Validation.Validators;

public class CPFValidTest
{
    [Fact]
    public void Should_Return_True_When_CPF_Is_Valid()
    {
        var cpf = "88917605010";

        var result = CPFValid.IsValid(cpf);

        Assert.True(result);
    }

    [Fact]
    public void Should_Return_False_When_CPF_Is_Invalid()
    {
        var cpf = "88927605010";

        var result = CPFValid.IsValid(cpf);

        Assert.False(result);
    }

    [Fact]
    public void Should_Return_False_When_CPF_Has_Letter()
    {
        var cpf = "8891760501A";

        var result = CPFValid.IsValid(cpf);

        Assert.False(result);
    }

    [Fact]
    public void Should_Return_False_When_CPF_Has_Same_Digits()
    {
        var cpf = "11111111111";

        var result = CPFValid.IsValid(cpf);

        Assert.False(result);
    }

    [Fact]
    public void Should_Return_False_When_CPF_Has_Less_Than_11_Digits()
    {
        var cpf = "6871424709";

        var result = CPFValid.IsValid(cpf);

        Assert.False(result);
    }

    [Fact]
    public void Should_Return_False_When_CPF_Has_More_Than_11_Digits()
    {
        var cpf = "68714247091";

        var result = CPFValid.IsValid(cpf);

        Assert.False(result);
    }

    [Fact]
    public void Should_Return_False_When_First_Digit_Is_Invalid()
    {
        var cpf = "68714247087";

        var result = CPFValid.IsValid(cpf);

        Assert.False(result);
    }

    [Fact]
    public void Should_Return_False_When_Second_Digit_Is_Invalid()
    {
        var cpf = "68714247096";

        var result = CPFValid.IsValid(cpf);

        Assert.False(result);
    }
}