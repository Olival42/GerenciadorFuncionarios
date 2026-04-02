using Xunit;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;

public class UpdateEstoqueDTOTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true);

        return results;
    }

    [Fact]
    public void UpdateEstoqueDTO_Should_Be_Valid()
    {
        var dto = new UpdateEstoqueDTO
        {
            Quantity = 10
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void UpdateEstoqueDTO_Should_Be_Valid_When_Quantity_Zero()
    {
        var dto = new UpdateEstoqueDTO
        {
            Quantity = 0
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void UpdateEstoqueDTO_Should_Be_Valid_When_Quantity_Negative()
    {
        var dto = new UpdateEstoqueDTO
        {
            Quantity = -10
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }

    [Fact]
    public void UpdateEstoqueDTO_Should_Be_Valid_When_Quantity_MaxValue()
    {
        var dto = new UpdateEstoqueDTO
        {
            Quantity = int.MaxValue
        };

        var result = ValidateModel(dto);

        Assert.Empty(result);
    }
}