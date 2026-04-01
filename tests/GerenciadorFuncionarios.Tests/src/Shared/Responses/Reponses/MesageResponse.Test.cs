using Xunit;
using GerenciadorFuncionarios.Shared.Responses;

public class MessageResponseTests
{
    [Fact]
    public void Should_Create_Error_Response_With_Correct_Values()
    {
        var message = "CPF é inválido";

        var result = new MessageResponse(message);

        Assert.Equal(message, result.Message);
    }
}