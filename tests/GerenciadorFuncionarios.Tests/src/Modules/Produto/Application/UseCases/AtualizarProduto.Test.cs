using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

public class AtualizarProdutoUseCaseTests
{
    private readonly Mock<IProdutoRepository> _mockRepository;

    private readonly AtualizarProduto _useCase;

    public AtualizarProdutoUseCaseTests()
    {
        _mockRepository = new Mock<IProdutoRepository>();

        _useCase = new AtualizarProduto(_mockRepository.Object);
    }

    private Produto Create_Produto()
    {
        return new Produto
        {
            Id = Guid.NewGuid(),
            Name = "Gas 12kg",
            Type = TipoProduto.GAS,
            Quantity = 50,
            Price = 110,
            IsActive = true
        };
    }

    [Fact]
    public async Task Execute_Should_Update_Name()
    {
        var prod = Create_Produto();

        _mockRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        var result = await _useCase.Execute(prod.Id, new UpdateProdutoDTO { Name = "Gas 14kg" });

        Assert.Equal("Gas 14kg", result.Data!.Name);
        _mockRepository.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Execute_Should_Update_Price()
    {
        var prod = Create_Produto();

        _mockRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        var result = await _useCase.Execute(prod.Id, new UpdateProdutoDTO { Price = 120 });

        Assert.Equal(120.0, result.Data!.Price);
        _mockRepository.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Execute_Should_Not_Change_Quantity()
    {
        var prod = Create_Produto();

        _mockRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        var result = await _useCase.Execute(prod.Id, new UpdateProdutoDTO { Name = "Gas 14kg" });

        Assert.Equal(prod.Quantity, result.Data!.Quantity);
    }

    [Fact]
    public async Task Execute_Should_Update_Only_Provided_Fields()
    {
        var prod = Create_Produto();

        var originalPrice = prod.Price;

        _mockRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        await _useCase.Execute(prod.Id, new UpdateProdutoDTO { Name = "Novo" });

        Assert.Equal("Novo", prod.Name);
        Assert.Equal(originalPrice, prod.Price);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_Product_Not_Found()
    {
        var dto = new UpdateProdutoDTO { Name = "Gas 14kg" };

        _mockRepository.Setup(p => p.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _useCase.Execute(Guid.NewGuid(), dto));

        Assert.Equal("Produto não encontrado.", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_Not_Call_SaveChanges_When_Not_Found()
    {
        var dto = new UpdateProdutoDTO { Name = "Gas 14kg" };

        _mockRepository.Setup(p => p.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _useCase.Execute(Guid.NewGuid(), dto));

        _mockRepository.Verify(p => p.SaveChangesAsync(), Times.Never);
    }
}
