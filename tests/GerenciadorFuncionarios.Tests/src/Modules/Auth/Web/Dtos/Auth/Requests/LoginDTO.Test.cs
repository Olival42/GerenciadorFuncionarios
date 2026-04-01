using Xunit;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Modules.Auth.Web.Requests;

public class LoginDTOTests
{
    [Fact]
    public void LoginDTO_Should_Be_Valid_When_Fields_Are_Filled()
    {
        var dto = new LoginDTO{ Email = "test@email.com", Password = "123456"};

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void LoginDTO_Should_Return_Error_When_Email_Is_Null()
    {
        var dto = new LoginDTO{Email = null!, Password = "123456"};

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Email é obrigatório");
    }

    [Fact]
    public void LoginDTO_Should_Return_Error_When_Password_Is_Null()
    {
        var dto = new LoginDTO{Email = "test@email.com", Password = null!};

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Senha é obrigatório");
    }

    [Fact]
    public void LoginDTO_Should_Return_Two_Errors_When_All_Invalid()
    {
        var dto = new LoginDTO{Email = null!, Password = null!};

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Equal(2, results.Count);
    }
}