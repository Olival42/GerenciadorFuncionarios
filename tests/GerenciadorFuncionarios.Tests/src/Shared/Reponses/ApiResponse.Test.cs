using GerenciadorFuncionarios.Shared.Responses;
using Xunit;

public class ApiReponseTest
{
    [Fact]
    public void Should_Create_Success_Response_When_Ok_Is_Called()
    {
        var data = "dados";

        var result = ApiResponse<string>.Ok(data);

        Assert.True(result.Success);
        Assert.Equal(data, result.Data);
        Assert.Null(result.Error);
        Assert.True(result.Timestamp <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Should_Create_Fail_Response_When_Fail_Is_Called()
    {
        var error = new ErrorResponse("codigo", "mensagem");

        var result = ApiResponse<string>.Fail(error);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(error, result.Error);
        Assert.True(result.Timestamp <= DateTimeOffset.UtcNow);
    }
}