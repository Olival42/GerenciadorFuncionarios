using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;
using GerenciadorFuncionarios.Domain.ValueObjects;

public class AtualizarFuncionarioUseCaseTests
{
    private readonly Mock<IFuncionarioRepository> _mockRepository;

    private readonly AtualizarFuncionario _useCase;

    public AtualizarFuncionarioUseCaseTests()
    {
        _mockRepository = new Mock<IFuncionarioRepository>();

        _useCase = new AtualizarFuncionario(_mockRepository.Object);
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
    public async Task Execute_Should_UpdateAllFields_WhenAllProvided()
    {
        var func = Create_Funcionario();

        var request = new UpdateFuncionarioDTO
        {
            Name = "rogerio",
            UserName = "rogerio",
            Password = new Password("StrongP@ss2"),
            Role = Role.GERENTE,
            Phone = new Phone("44888888888"),
            CPF = new Cpf("78945612300")
        };

        _mockRepository.
            Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _useCase.Execute(func.Id, request);

        Assert.Equal(request.Name, func.Name);
        Assert.Equal(request.UserName, func.UserName);
        Assert.True(BCrypt.Net.BCrypt.Verify(request.Password, func.PasswordHash));
        Assert.Equal(request.Role, func.Role);
        Assert.Equal(request.Phone, func.Phone);
        Assert.Equal(request.CPF, func.CPF);
    }

    [Fact]
    public async Task Execute_Should_UpdateOnlyProvidedFields()
    {
        var func = Create_Funcionario();

        var original = new
        {
            func.Name,
            func.UserName,
            func.Role,
            func.Phone,
            func.CPF,
            func.PasswordHash
        };

        var request = new UpdateFuncionarioDTO
        {
            Name = "rogerio",
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        await _useCase.Execute(func.Id, request);

        Assert.Equal("rogerio", func.Name);
        Assert.Equal(original.UserName, func.UserName);
        Assert.Equal(original.Role, func.Role);
        Assert.Equal(original.Phone, func.Phone);
        Assert.Equal(original.CPF, func.CPF);
        Assert.Equal(original.PasswordHash, func.PasswordHash);
    }

    [Fact]
    public async Task Execute_Should_Throw_WhenFuncionarioNotFound()
    {
        var request = new UpdateFuncionarioDTO
        {
            Name = "rogerio",
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Funcionario?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _useCase.Execute(Guid.NewGuid(), request));

        Assert.Equal("Funcionário não encontrado.", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_CallSaveChanges_WhenValid()
    {
        var func = Create_Funcionario();

        var request = new UpdateFuncionarioDTO
        {
            Name = "rogerio",
        };

        _mockRepository.
            Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _useCase.Execute(func.Id, request);

        _mockRepository.Verify(
           f => f.SaveChangesAsync(),
           Times.Once
        );
    }

    [Fact]
    public async Task Execute_Should_CallRepository_WithCorrectId()
    {
        var func = Create_Funcionario();

        var request = new UpdateFuncionarioDTO
        {
            Name = "rogerio",
        };

        _mockRepository.
            Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _useCase.Execute(func.Id, request);

        _mockRepository.Verify(
           f => f.GetByIdAsync(func.Id),
           Times.Once
        );
    }

    [Fact]
    public async Task Execute_Should_NotChangeFields_WhenNull()
    {
        var func = Create_Funcionario();

        var original = new
        {
            func.Name,
            func.UserName,
            func.Role,
            func.Phone,
            func.CPF,
            func.PasswordHash
        };

        var request = new UpdateFuncionarioDTO
        {
            Name = "Admin",
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _useCase.Execute(func.Id, request);

        Assert.Equal(original.UserName, result.Data!.UserName);
        Assert.Equal(original.Role, result.Data.Role);
        Assert.Equal(original.Phone, result.Data.Phone);
        Assert.Equal(original.CPF, result.Data.CPF);
        Assert.Equal(original.PasswordHash, func.PasswordHash);
    }
}
