using Xunit;
using GerenciadorFuncionarios.Shared.Responses;

public class DataErrorTests
{
    [Fact]
    public void Should_Create_Error_Response_With_Correct_Values()
    {
        var field = "CPF";
        var message = "CPF é inválido";

        var result = new DataErrors(field, message);

        Assert.Equal(field, result.Field);
        Assert.Equal(message, result.Message);
    }
}