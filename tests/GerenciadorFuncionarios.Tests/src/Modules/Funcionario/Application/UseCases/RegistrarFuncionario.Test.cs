using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;
using GerenciadorFuncionarios.Domain.ValueObjects;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Exceptions;

public class RegistrarFuncionarioUseCaseTests
{
    private readonly Mock<IFuncionarioRepository> _mockRepository;

    private readonly RegistrarFuncionario _useCase;

    public RegistrarFuncionarioUseCaseTests()
    {
        _mockRepository = new Mock<IFuncionarioRepository>();

        _useCase = new RegistrarFuncionario(_mockRepository.Object);
    }

    private RegisterFuncionarioDTO Create_RegisterDto()
    {
        var cpf = new Cpf("12345678900");
        var phone = new Phone("44999999999");
        var password = new Password("StrongP@ss1");

        return new RegisterFuncionarioDTO
        {
            Name = "Admin",
            UserName = "admin",
            Password = password,
            Role = Role.GERENTE,
            CPF = cpf,
            Phone = phone,
        };
    }

    [Fact]
    public async Task Execute_Should_Register_When_Data_Is_Valid()
    {
        var dto = Create_RegisterDto();

        _mockRepository
            .Setup(r => r.AnyByCPFAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AnyByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _useCase.Execute(dto);

        Assert.True(result.Success);

        _mockRepository.Verify(
            r => r.Add(It.IsAny<Funcionario>()),
            Times.Once);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_CPF_Exists()
    {
        var dto = Create_RegisterDto();

        _mockRepository
            .Setup(r => r.AnyByCPFAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<CPFAlreadyExistsException>(
            () => _useCase.Execute(dto));

        Assert.Equal("CPF já cadastrado para outro funcionário.", exception.Message);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_UserName_Exists()
    {
        var dto = Create_RegisterDto();

        _mockRepository
            .Setup(r => r.AnyByCPFAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AnyByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<UserNameAlreadyExistsException>(
            () => _useCase.Execute(dto));

        Assert.Equal("Nome do usuário já cadastrado para outro funcionário.", exception.Message);
    }

    [Fact]
    public async Task Execute_Should_HashPassword()
    {
        var dto = Create_RegisterDto();
        Funcionario func = null!;

        _mockRepository
            .Setup(r => r.AnyByCPFAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AnyByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.Add(It.IsAny<Funcionario>()))
            .Callback<Funcionario>(f => func = f)
            .Returns(Task.CompletedTask);

        await _useCase.Execute(dto);

        Assert.NotNull(func);
        Assert.NotEqual(dto.Password, func.PasswordHash);
        Assert.True(
            BCrypt.Net.BCrypt.Verify(dto.Password, func.PasswordHash)
        );
    }

    [Fact]
    public async Task Execute_Should_Return_DTO()
    {
        var funcDto = Create_RegisterDto();

        _mockRepository
            .Setup(r => r.AnyByCPFAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AnyByUserNameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _useCase.Execute(funcDto);

        Assert.NotNull(result);
        Assert.True(result.Success);

        Assert.Equal(funcDto.Name, result.Data!.Name);
        Assert.Equal(funcDto.UserName, result.Data!.UserName);
        Assert.Equal(funcDto.Phone, result.Data!.Phone);
        Assert.Equal(funcDto.CPF, result.Data!.CPF);
        Assert.Equal(funcDto.Role, result.Data!.Role);
    }
}
