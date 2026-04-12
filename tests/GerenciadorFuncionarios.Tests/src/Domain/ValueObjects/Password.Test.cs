using System;
using GerenciadorFuncionarios.Domain.ValueObjects;
using Xunit;

public class PasswordTests
{
    [Theory]
    [InlineData("Aa1@abcd")]
    [InlineData("P@ssw0rd1")]
    [InlineData("StrongPass#9")]
    public void Password_Should_Construct_With_Valid_Value(string value)
    {
        var password = new Password(value);

        Assert.Equal(value, password.Value);
        Assert.Equal(value, (string)password);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Password_Should_Throw_When_Empty_Or_Null(string value)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Password(value!));
        Assert.Equal("Senha não pode ser vazia.", ex.Message);
    }

    [Theory]
    [InlineData("short1A@")]
    [InlineData("nouppercase1@")]
    [InlineData("NOLOWERCASE1@")]
    [InlineData("NoNumber@@")]
    [InlineData("NoSpecial123")]
    public void Password_Should_Throw_When_Requirements_Not_Met(string value)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Password(value));
        Assert.Equal(
            "A senha deve conter no mínimo 8 caracteres, uma letra maiúscula, uma minúscula, um número e um caractere especial.",
            ex.Message);
    }
}
