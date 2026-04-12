using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;
using GerenciadorFuncionarios.Domain.ValueObjects;

public class ObterFuncionarioPorIdUseCaseTests
{
    private readonly Mock<IFuncionarioRepository> _mockRepository;

    private readonly ObterFuncionarioPorId _useCase;

    public ObterFuncionarioPorIdUseCaseTests()
    {
        _mockRepository = new Mock<IFuncionarioRepository>();

        _useCase = new ObterFuncionarioPorId(_mockRepository.Object);
    }

    private Funcionario Create_Funcionario()
    {
        var cpf = new Cpf("68714247097");
        var phone = new Phone("44999999999");
        var password = new Password("StrongP@ss1");

        return new Funcionario
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Phone = phone,
            CPF = cpf,
            UserName = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword((string)password),
            Role = Role.GERENTE,
            IsActive = true
        };
    }

    [Fact]
    public async Task Execute_Should_ReturnFuncionario_WhenExists()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _useCase.Execute(func.Id);

        Assert.NotNull(result);
        Assert.True(result.Success);

        Assert.Equal(func.Id, result.Data!.Id);
        Assert.Equal(func.Name, result.Data!.Name);
        Assert.Equal(func.UserName, result.Data!.UserName);
        Assert.Equal(func.Phone, result.Data!.Phone);
        Assert.Equal(func.CPF, result.Data!.CPF);
        Assert.Equal(func.Role, result.Data!.Role);
        Assert.Equal(func.IsActive, result.Data!.IsActive);
    }

    [Fact]
    public async Task Execute_Should_Throw_WhenNotFound()
    {
        var id = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Funcionario?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _useCase.Execute(id)
        );

        Assert.Equal("Funcionário não encontrado.", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_CallRepository_WithCorrectId()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _useCase.Execute(func.Id);

        _mockRepository.Verify(
            r => r.GetByIdAsync(func.Id),
            Times.Once);
    }

    [Fact]
    public async Task Execute_Should_ReturnSuccessTrue()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _useCase.Execute(func.Id);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Execute_Should_MapFieldsCorrectly()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _useCase.Execute(func.Id);

        Assert.Equal(func.Id, result.Data!.Id);
        Assert.Equal(func.Name, result.Data!.Name);
        Assert.Equal(func.UserName, result.Data!.UserName);
        Assert.Equal(func.Phone, result.Data!.Phone);
        Assert.Equal(func.CPF, result.Data!.CPF);
        Assert.Equal(func.Role, result.Data!.Role);
        Assert.Equal(func.IsActive, result.Data!.IsActive);
    }
}
