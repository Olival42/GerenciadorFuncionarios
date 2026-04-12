using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

public class ObterProdutoPorIdUseCaseTests
{
    private readonly Mock<IProdutoRepository> _mockRepository;

    private readonly ObterProdutoPorId _useCase;

    public ObterProdutoPorIdUseCaseTests()
    {
        _mockRepository = new Mock<IProdutoRepository>();

        _useCase = new ObterProdutoPorId(_mockRepository.Object);
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
    public async Task Execute_Should_Return_Product_When_Exists()
    {
        var prod = Create_Produto();
        var id = prod.Id;

        _mockRepository.Setup(p => p.GetByIdAsync(id)).ReturnsAsync(prod);

        var result = await _useCase.Execute(id);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(id, result.Data!.Id);
        Assert.Equal(prod.Name, result.Data.Name);
        Assert.Equal(prod.Price, result.Data.Price);
        Assert.Equal(prod.Quantity, result.Data.Quantity);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_Not_Found()
    {
        _mockRepository.Setup(p => p.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _useCase.Execute(Guid.NewGuid()));

        Assert.Equal("Produto não encontrado.", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_Return_Mapped_DTO()
    {
        var prod = Create_Produto();
        var id = prod.Id;

        _mockRepository.Setup(p => p.GetByIdAsync(id)).ReturnsAsync(prod);

        var result = await _useCase.Execute(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Data!.Id);
        Assert.Equal(prod.Name, result.Data.Name);
        Assert.Equal(prod.Price, result.Data.Price);
        Assert.Equal(prod.Quantity, result.Data.Quantity);
        Assert.True(result.Data.IsActive);
    }

    [Fact]
    public async Task Execute_Should_Return_ApiResponse()
    {
        var prod = Create_Produto();
        var id = prod.Id;

        _mockRepository.Setup(p => p.GetByIdAsync(id)).ReturnsAsync(prod);

        var result = await _useCase.Execute(id);

        Assert.IsType<ApiResponse<ResponseProdutoDTO>>(result);
        Assert.True(result.Success);
        Assert.Equal(id, result.Data!.Id);
        Assert.Equal(result.Data.Name, prod.Name);
        Assert.Equal(result.Data.Price, prod.Price);
        Assert.Equal(result.Data.Quantity, prod.Quantity);
    }
}
