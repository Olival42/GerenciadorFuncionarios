using Xunit;
using GerenciadorFuncionarios.Shared.Responses;

public class PaginationResponseTests
{
    [Fact]
    public void Should_Calculate_Total_Pages_orrectly()
    {
        var response = new PaginationResponse<int>(
            Items: new List<int>(),
            Page: 1,
            PageSize: 10,
            TotalItems: 25
        );

        Assert.Equal(3, response.TotalPages);
    }

    [Fact]
    public void Should_Return_False_When_Not_Last_Page()
    {
        var response = new PaginationResponse<int>(
            new List<int>(),
            Page: 1,
            PageSize: 10,
            TotalItems: 25
        );

        Assert.False(response.IsLastPage);
    }

    [Fact]
    public void Should_Return_True_When_Last_Page()
    {
        var response = new PaginationResponse<int>(
            new List<int>(),
            Page: 3,
            PageSize: 10,
            TotalItems: 25
        );

        Assert.True(response.IsLastPage);
    }

    [Fact]
    public void Should_Calculate_Total_Pages_When_Exact_Division()
    {
        var response = new PaginationResponse<int>(
            new List<int>(),
            Page: 1,
            PageSize: 10,
            TotalItems: 20
        );

        Assert.Equal(2, response.TotalPages);
    }

    [Fact]
    public void Should_Return_Zero_Pages_When_No_Items()
    {
        var response = new PaginationResponse<int>(
            new List<int>(),
            Page: 1,
            PageSize: 10,
            TotalItems: 0
        );

        Assert.Equal(0, response.TotalPages);
    }
}