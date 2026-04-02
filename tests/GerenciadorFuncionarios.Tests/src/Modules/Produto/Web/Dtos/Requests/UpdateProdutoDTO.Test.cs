using Xunit;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;

public class UpdateProdutoDTOTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true);

        return results;
    }

    [Fact]
    public void UpdateProdutoDTO_Should_Be_Valid_When_All_Null()
    {
        var dto = new UpdateProdutoDTO();

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void UpdateProdutoDTO_Should_Be_Valid_When_Only_Name()
    {
        var dto = new UpdateProdutoDTO
        {
            Name = "Gas 13kg"
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void UpdateProdutoDTO_Should_Be_Valid_When_Only_Price()
    {
        var dto = new UpdateProdutoDTO
        {
            Price = 100
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void UpdateProdutoDTO_Should_Return_Error_When_Name_Too_Long()
    {
        var dto = new UpdateProdutoDTO
        {
            Name = new string('A', 101)
        };

        var result = ValidateModel(dto);

        Assert.Contains(result, r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void UpdateProdutoDTO_Should_Return_Error_When_Price_Zero()
    {
        var dto = new UpdateProdutoDTO
        {
            Price = 0
        };

        var result = ValidateModel(dto);

        Assert.Contains(result, r => r.MemberNames.Contains("Price"));
    }

    [Fact]
    public void UpdateProdutoDTO_Should_Return_Error_When_Price_Negative()
    {
        var dto = new UpdateProdutoDTO
        {
            Price = -10
        };

        var result = ValidateModel(dto);

        Assert.Contains(result, r => r.MemberNames.Contains("Price"));
    }

    [Fact]
    public void UpdateProdutoDTO_Should_Be_Valid_When_Name_Max_Length()
    {
        var dto = new UpdateProdutoDTO
        {
            Name = new string('A', 100)
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void UpdateProdutoDTO_Should_Be_Valid_When_Price_Minimum()
    {
        var dto = new UpdateProdutoDTO
        {
            Price = 0.01
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void UpdateProdutoDTO_Should_Be_Valid_When_Both_Fields()
    {
        var dto = new UpdateProdutoDTO
        {
            Name = "Gas 13kg",
            Price = 150
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }
}