using Xunit;
using GerenciadorFuncionarios.Validation.Attributes;
using System.ComponentModel.DataAnnotations;

public class CPFAttributeTest
{
    [Fact]
    public void Should_Return_Sucess_When_CPF_Is_Valid()
    {
        var attribute = new CPFValidAttribute();

        var context = new ValidationContext(new object());

        var result = attribute.GetValidationResult("68714247097", context);

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void Should_Return_Error_Message_When_CPF_Is_Invalid()
    {
        var attribute = new CPFValidAttribute();

        var context = new ValidationContext(new object());

        var result = attribute.GetValidationResult("68714247091", context);

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("CPF é inválido.", result!.ErrorMessage);
    }

    [Fact]
    public void Should_Return_Error_Message_When_Value_Is_Not_String()
    {
        var attribute = new CPFValidAttribute();

        var context = new ValidationContext(new object());

        var result = attribute.GetValidationResult(123, context);

        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("Tipo inválido para validação.", result!.ErrorMessage);
    }
}