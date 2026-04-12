using System;
using GerenciadorFuncionarios.Domain.ValueObjects;
using Xunit;

public class PhoneTests
{
    [Theory]
    [InlineData("11912345678", "11912345678")]
    [InlineData("(11) 91234-5678", "11912345678")]
    [InlineData("11 91234 5678", "11912345678")]
    [InlineData("1191234567", "1191234567")]
    public void Telefone_Should_Construct_And_Normalize(string input, string expected)
    {
        var phone = new Phone(input);

        Assert.Equal(expected, phone.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Telefone_Should_Throw_When_Empty_Or_Null(string value)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Phone(value!));
        Assert.Equal("Telefone não pode ser vazio.", exception.Message);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("abcd567890")]
    [InlineData("119123456")]
    [InlineData("119123456789")]
    public void Telefone_Should_Throw_When_Invalid(string value)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Phone(value));
        Assert.Equal("Telefone inválido.", exception.Message);
    }
}
