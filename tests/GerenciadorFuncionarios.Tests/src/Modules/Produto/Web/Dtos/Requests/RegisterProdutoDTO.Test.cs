using Xunit;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;

public class RegisterProdutoDTOTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model, null, null);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true);

        return results;
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Be_Valid()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas 12kg",
            Type = TipoProduto.GAS,
            Quantity = 10,
            Price = 100
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Return_Error_When_Name_Empty()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "",
            Type = TipoProduto.GAS,
            Quantity = 10,
            Price = 100
        };

        var result = ValidateModel(dto);

        Assert.Contains(result, r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Return_Error_When_Name_Too_Long()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = new string('A', 101),
            Type = TipoProduto.GAS,
            Quantity = 10,
            Price = 100
        };

        var result = ValidateModel(dto);

        Assert.Contains(result, r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Return_Error_When_Quantity_Zero()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas",
            Type = TipoProduto.GAS,
            Quantity = 0,
            Price = 100
        };

        var result = ValidateModel(dto);

        Assert.Contains(result, r => r.MemberNames.Contains("Quantity"));
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Return_Error_When_Quantity_Negative()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas",
            Type = TipoProduto.GAS,
            Quantity = -1,
            Price = 100
        };

        var result = ValidateModel(dto);

        Assert.Contains(result, r => r.MemberNames.Contains("Quantity"));
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Return_Error_When_Price_Zero()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas",
            Type = TipoProduto.GAS,
            Quantity = 10,
            Price = 0
        };

        var result = ValidateModel(dto);

        Assert.Contains(result, r => r.MemberNames.Contains("Price"));
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Return_Error_When_Price_Negative()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas",
            Type = TipoProduto.GAS,
            Quantity = 10,
            Price = -10
        };

        var result = ValidateModel(dto);

        Assert.Contains(result, r => r.MemberNames.Contains("Price"));
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Be_Valid_When_Name_At_Max_Length()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = new string('A', 100),
            Type = TipoProduto.GAS,
            Quantity = 10,
            Price = 100
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Be_Valid_When_Quantity_Minimum()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas",
            Type = TipoProduto.GAS,
            Quantity = 1,
            Price = 100
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Be_Valid_When_Price_Minimum()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas",
            Type = TipoProduto.GAS,
            Quantity = 10,
            Price = 0.01
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Be_Valid_When_Name_With_Spaces()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "   Gas 12kg   ",
            Type = TipoProduto.GAS,
            Quantity = 10,
            Price = 100
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Be_Valid_With_Large_Values()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas",
            Type = TipoProduto.GAS,
            Quantity = int.MaxValue,
            Price = double.MaxValue
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Return_Error_When_Type_Invalid()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas",
            Type = (TipoProduto)999,
            Quantity = 10,
            Price = 100
        };

        var result = ValidateModel(dto);

        Assert.Contains(result, r => r.MemberNames.Contains("Type"));
    }

    [Fact]
    public void RegisterProdutoDTO_Should_Be_Valid_When_Type_Valid()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas",
            Type = TipoProduto.GAS,
            Quantity = 10,
            Price = 100
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }
}