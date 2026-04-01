using Xunit;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;

public class RegisterFuncionarioDTOTests
{
    [Fact]
    public void RegisterFuncionarioDTO_Should_Be_Valid_When_All_Fields_Are_Correct()
    {
        var dto = new RegisterFuncionarioDTO
        {
            Name = "João da Silva",
            Email = "joao@email.com",
            Password = "Senha@123",
            Role = Role.GERENTE,
            Phone = "44999999999",
            CPF = "52998224725",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Should_Return_Error_When_Name_Is_Null()
    {
        var dto = new RegisterFuncionarioDTO
        {
            Name = null!,
            Email = "joao@email.com",
            Password = "Senha@123",
            Role = Role.GERENTE,
            Phone = "44999999999",
            CPF = "52998224725",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Nome é obrigatório");
    }

    [Fact]
    public void Should_Return_Error_When_Email_Is_Invalid()
    {
        var dto = new RegisterFuncionarioDTO
        {
            Name = "João da Silva",
            Email = "joaoemail.com",
            Password = "Senha@123",
            Role = Role.GERENTE,
            Phone = "44999999999",
            CPF = "52998224725",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Email inválido");
    }

    [Fact]
    public void Should_Return_Error_When_Password_Is_Too_Short()
    {
        var dto = new RegisterFuncionarioDTO
        {
            Name = "João da Silva",
            Email = "joao@email.com",
            Password = "Senha@1",
            Role = Role.GERENTE,
            Phone = "44999999999",
            CPF = "52998224725",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "A senha deve ter no mínimo 8 caracteres");
    }

    [Fact]
    public void Should_Return_Error_When_Password_Invalid_Format()
    {
        var dto = new RegisterFuncionarioDTO
        {
            Name = "João da Silva",
            Email = "joao@email.com",
            Password = "Senha1123",
            Role = Role.GERENTE,
            Phone = "44999999999",
            CPF = "52998224725",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um dígito e um caractere especial.");
    }

    [Fact]
    public void Should_Return_Error_When_Phone_Invalid()
    {
        var dto = new RegisterFuncionarioDTO
        {
            Name = "João da Silva",
            Email = "joao@email.com",
            Password = "Senha@123",
            Role = Role.GERENTE,
            Phone = "449999999",
            CPF = "52998224725",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Telefone inválido. Formato esperado: 99999999999");
    }

    [Fact]
    public void Should_Return_Error_When_CPF_Invalid()
    {
        var dto = new RegisterFuncionarioDTO
        {
            Name = "João da Silva",
            Email = "joao@email.com",
            Password = "Senha@123",
            Role = Role.GERENTE,
            Phone = "44999999999",
            CPF = "52998224724",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "CPF é inválido.");
    }

    [Fact]
    public void RegisterFuncionarioDTO_Should_Return_Error_When_Role_Invalid()
    {
        var dto = new RegisterFuncionarioDTO
        {
            Name = "João da Silva",
            Email = "joao@email.com",
            Password = "Senha@123",
            Role = (Role)999,
            Phone = "44999999999",
            CPF = "52998224725",
        };

        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            dto,
            new ValidationContext(dto),
            validationResults,
            validateAllProperties: true
        );

        Assert.False(isValid);
        Assert.Contains(validationResults, r => r.ErrorMessage == "Cargo inválido");
    }
}