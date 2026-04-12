using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

public class InativarProdutoUseCaseTests
{
    private readonly Mock<IProdutoRepository> _mockRepository;

    private readonly InativarProduto _useCase;

    public InativarProdutoUseCaseTests()
    {
        _mockRepository = new Mock<IProdutoRepository>();

        _useCase = new InativarProduto(_mockRepository.Object);
    }

    private Produto Create_Produto()
    {
        return new Produto
        {
            Id = Guid.NewGuid(),
            Name = "Gas 12kg",
            Type = TipoProduto.GAS,
            Quantity = 0,
            Price = 110,
            IsActive = true
        };
    }

    [Fact]
    public async Task Execute_Should_Inactivate_Product_When_Quantity_Zero()
    {
        var prod = Create_Produto();
        var id = prod.Id;

        _mockRepository
            .Setup(p => p.GetByIdAsync(id))
            .ReturnsAsync(prod);

        await _useCase.Execute(id);

        Assert.False(prod.IsActive);

        _mockRepository.Verify(
            p => p.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task Execute_Should_Set_IsActive_False()
    {
        var prod = Create_Produto();

        _mockRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        await _useCase.Execute(prod.Id);

        Assert.False(prod.IsActive);
        _mockRepository.Verify(p => p.SaveChangesAsync());
    }

    [Fact]
    public async Task Execute_Should_Throw_When_Product_Not_Found()
    {
        _mockRepository.Setup(p => p.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _useCase.Execute(Guid.NewGuid()));

        Assert.Equal("Produto não encontrado.", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_Product_Has_Stock()
    {
        var prod = Create_Produto();
        prod.Quantity = 10;

        _mockRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.Execute(prod.Id));

        Assert.Equal("Produto ainda tem estoque, não foi possível inativá-lo.", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_Not_Call_SaveChanges_When_Not_Found()
    {
        _mockRepository.Setup(p => p.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _useCase.Execute(Guid.NewGuid()));

        _mockRepository.Verify(p => p.SaveChangesAsync(), Times.Never);
    }
}
