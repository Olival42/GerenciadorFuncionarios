using Xunit;
using GerenciadorFuncionarios.Shared.Responses;

public class ErrorResponseTests
{
    [Fact]
    public void Should_Create_Error_Response_With_Correct_Values()
    {
        var code = "CPF_INVALIDO";
        var message = "CPF é inválido";

        var result = new ErrorResponse(code, message);

        Assert.Equal(code, result.Code);
        Assert.Equal(message, result.Message);
    }
}