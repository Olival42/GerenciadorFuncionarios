using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;
using GerenciadorFuncionarios.Domain.ValueObjects;

public class InativarFuncionarioUseCaseTests
{
    private readonly Mock<IFuncionarioRepository> _mockRepository;

    private readonly InativarFuncionario _useCase;

    public InativarFuncionarioUseCaseTests()
    {
        _mockRepository = new Mock<IFuncionarioRepository>();

        _useCase = new InativarFuncionario(_mockRepository.Object);
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
    public async Task Execute_Should_SetIsActiveFalse_WhenFuncionarioExists()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        await _useCase.Execute(func.Id);

        Assert.False(func.IsActive);
    }

    [Fact]
    public async Task Execute_Should_CallSaveChanges_WhenFuncionarioExists()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        await _useCase.Execute(func.Id);

        _mockRepository.Verify(
            r => r.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task Execute_Should_Throw_WhenFuncionarioNotFound()
    {
        var id = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Funcionario?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _useCase.Execute(id));

        Assert.Equal("Funcionário não encontrado.", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_CallRepository_WithCorrectId()
    {
        var func = Create_Funcionario();

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        await _useCase.Execute(func.Id);

        _mockRepository.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>()),
            Times.Once);
    }

    [Fact]
    public async Task Execute_Should_NotCallSaveChanges_WhenFuncionarioNotFound()
    {
        var id = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Funcionario?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _useCase.Execute(id));

        _mockRepository.Verify(
            r => r.SaveChangesAsync(),
            Times.Never);
    }
}
