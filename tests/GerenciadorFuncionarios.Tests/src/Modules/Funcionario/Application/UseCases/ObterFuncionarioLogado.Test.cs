using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Models;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Modules.Auth.Application.Services;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;
using GerenciadorFuncionarios.Domain.ValueObjects;

public class ObterFuncionarioLogadoUseCaseTests
{
    private readonly Mock<IFuncionarioRepository> _mockRepository;
    private readonly Mock<IUserContextService> _mockUserContext;

    private readonly ObterFuncionarioLogado _useCase;

    public ObterFuncionarioLogadoUseCaseTests()
    {
        _mockRepository = new Mock<IFuncionarioRepository>();
        _mockUserContext = new Mock<IUserContextService>();

        _useCase = new ObterFuncionarioLogado(
            _mockRepository.Object,
            _mockUserContext.Object
        );
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
    public async Task Execute_Should_ReturnFuncionario_When_Authenticated()
    {
        var func = Create_Funcionario();
        _mockUserContext.Setup(u => u.GetUserId()).Returns(func.Id.ToString());
        _mockRepository.Setup(r => r.GetByIdAsync(func.Id)).ReturnsAsync(func);

        var result = await _useCase.Execute();

        Assert.True(result.Success);
        Assert.Equal(func.Id, result.Data!.Id);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_UserId_NotFound()
    {
        _mockUserContext.Setup(u => u.GetUserId()).Returns((string?)null);

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _useCase.Execute());

        Assert.Equal("Usuário não autenticado", exception.Message);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_UserId_InvalidFormat()
    {
        _mockUserContext.Setup(u => u.GetUserId()).Returns("invalid-guid");

        var exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _useCase.Execute());

        Assert.Equal("ID de usuário inválido", exception.Message);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_Funcionario_NotFound()
    {
        var id = Guid.NewGuid();
        _mockUserContext.Setup(u => u.GetUserId()).Returns(id.ToString());
        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Funcionario?)null);

        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _useCase.Execute());

        Assert.Equal("Funcionário não encontrado.", exception.Message);
    }
}
