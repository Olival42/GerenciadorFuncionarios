using Xunit;
using System.ComponentModel.DataAnnotations;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.Enums;

public class UpdateFuncionarioDTOTests
{
    [Fact]
    public void UpdateFuncionarioDTO_Should_Pass_When_Name_Is_Empty()
    {
        var dto = new UpdateFuncionarioDTO
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

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void UpdateFuncionarioDTO_Should_Return_Error_When_Name_Too_Long()
    {
        var dto = new UpdateFuncionarioDTO
        {
            Name = "swrwQcQXmuPFjpr%h&A#NZy4uarLx45ymJiP7uGpcqgt9Gwo@8u6vEVmUwcLveQjrWyPo!pp@626eVoGg94%8AWff9JV5CQhEaSxY",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Nome tem que ter no máximo 100 caracteres");
    }

    [Fact]
    public void UpdateFuncionarioDTO_Should_Pass_When_Email_Is_Empty()
    {
        var dto = new UpdateFuncionarioDTO
        {
            Name = "a",
            Email = null!,
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
    public void UpdateFuncionarioDTO_Should_Return_Error_When_Email_Invalid()
    {
        var dto = new UpdateFuncionarioDTO
        {
            Email = "joaosemail.com",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Email inválido");
    }

    [Fact]
    public void UpdateFuncionarioDTO_Should_Pass_When_Password_Is_Empty()
    {
        var dto = new UpdateFuncionarioDTO
        {
            Name = "nulls",
            Email = "joao@email.com",
            Password = null!,
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
    public void UpdateFuncionarioDTO_Should_Return_Error_When_Password_Too_Short()
    {
        var dto = new UpdateFuncionarioDTO
        {
            Password = "Senha@1",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "A senha deve ter no mínimo 8 caracteres e no máximo 20 caracteres");
    }

    [Fact]
    public void UpdateFuncionarioDTO_Should_Return_Error_When_Password_Too_Long()
    {
        var dto = new UpdateFuncionarioDTO
        {
            Password = "Senha@12345678901234567890",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "A senha deve ter no mínimo 8 caracteres e no máximo 20 caracteres");
    }

    [Fact]
    public void UpdateFuncionarioDTO_Should_Return_Error_When_Password_Weak()
    {
        var dto = new UpdateFuncionarioDTO
        {
            Password = "Senha1123",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "A senha deve conter pelo menos uma letra maiúscula, uma letra minúscula, um dígito e um caractere especial.");
    }

    [Fact]
    public void UpdateFuncionarioDTO_Should_Pass_When_Role_Is_Null()
    {
        var dto = new UpdateFuncionarioDTO
        {
            Name = "nulls",
            Email = "joao@email.com",
            Password = "Senha@123",
            Role = null,
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
    public void UpdateFuncionarioDTO_Should_Pass_When_Phone_Is_Empty()
    {
        var dto = new UpdateFuncionarioDTO
        {
            Name = "nulls",
            Email = "joao@email.com",
            Password = "Senha@123",
            Role = Role.GERENTE,
            Phone = null!,
            CPF = "52998224725",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void UpdateFuncionarioDTO_Should_Return_Error_When_Phone_Invalid()
    {
        var dto = new UpdateFuncionarioDTO
        {
            Phone = "449999999"
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "Telefone inválido. Formato esperado: 99999999999");
    }

    [Fact]
    public void UpdateFuncionarioDTO_Should_Pass_When_CPF_Is_Empty()
    {
        var dto = new UpdateFuncionarioDTO
        {
            Name = "nulls",
            Email = "joao@email.com",
            Password = "Senha@123",
            Role = Role.GERENTE,
            Phone = "44999999999",
            CPF = null!,
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void UpdateFuncionarioDTO_Should_Return_Error_When_CPF_Invalid()
    {
        var dto = new UpdateFuncionarioDTO
        {
            CPF = "52998224724",
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, context, results, true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.ErrorMessage == "CPF é inválido.");
    }

    [Fact]
    public void UpdateFuncionarioDTO_Should_Return_Error_When_Role_Invalid()
    {
        var dto = new UpdateFuncionarioDTO
        {
            Role = (Role)999
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