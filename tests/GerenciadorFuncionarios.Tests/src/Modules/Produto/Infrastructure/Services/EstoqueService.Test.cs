using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Modules.Produto.Web.Hubs;
using GerenciadorFuncionarios.Modules.Produto.Application.Services;

public class EstoqueServiceTests
{
    private readonly Mock<IProdutoRepository> _mockRepository;
    private readonly Mock<IHubContext<EstoqueHub>> _mockHub;
    private readonly Mock<IHubClients> _mockClients;
    private readonly Mock<IClientProxy> _mockClientProxy;

    private readonly EstoqueService _service;

    public EstoqueServiceTests()
    {
        _mockRepository = new Mock<IProdutoRepository>();
        _mockHub = new Mock<IHubContext<EstoqueHub>>();
        _mockClients = new Mock<IHubClients>();
        _mockClientProxy = new Mock<IClientProxy>();

        _mockClients
            .Setup(c => c.All)
            .Returns(_mockClientProxy.Object);

        _mockHub
            .Setup(h => h.Clients)
            .Returns(_mockClients.Object);

        _service = new EstoqueService(
            _mockRepository.Object,
            _mockHub.Object
        );
    }

    private Produto CreateProduto()
    {
        return new Produto
        {
            Id = Guid.NewGuid(),
            Name = "Gas 12kg",
            Type = TipoProduto.GAS,
            Quantity = 50,
            Price = 100,
            IsActive = true
        };
    }

    [Fact]
    public async Task EntradaEstoque_Should_Increase_Quantity()
    {
        var produto = CreateProduto();

        _mockRepository
            .Setup(r => r.GetByIdAsync(produto.Id))
            .ReturnsAsync(produto);

        var result = await _service.EntradaEstoque(produto.Id, 10);

        Assert.Equal(60, result.Quantity);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task EntradaEstoque_Should_Throw_When_Quantity_Invalid()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.EntradaEstoque(Guid.NewGuid(), 0));
    }

    [Fact]
    public async Task EntradaEstoque_Should_Throw_When_Product_Not_Found()
    {
        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Produto?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _service.EntradaEstoque(Guid.NewGuid(), 10));
    }

    [Fact]
    public async Task BaixarEstoque_Should_Decrease_Quantity()
    {
        var produto = CreateProduto();

        _mockRepository
            .Setup(r => r.GetByIdAsync(produto.Id))
            .ReturnsAsync(produto);

        var result = await _service.BaixarEstoque(produto.Id, 10);

        Assert.Equal(40, result.Quantity);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Throw_When_Stock_Insufficient()
    {
        var produto = CreateProduto();
        produto.Quantity = 5;

        _mockRepository
            .Setup(r => r.GetByIdAsync(produto.Id))
            .ReturnsAsync(produto);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.BaixarEstoque(produto.Id, 10));
    }

    [Fact]
    public async Task BaixarEstoque_Should_Throw_When_Quantity_Invalid()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.BaixarEstoque(Guid.NewGuid(), 0));
    }

    [Fact]
    public async Task BaixarEstoque_Should_Send_Alert_When_Low_Stock()
    {
        var produto = CreateProduto();
        produto.Quantity = 4;

        _mockRepository
            .Setup(r => r.GetByIdAsync(produto.Id))
            .ReturnsAsync(produto);

        _mockRepository
            .Setup(r => r.GetLowStockAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Produto> { produto });

        await _service.BaixarEstoque(produto.Id, 1);

        _mockClientProxy.Verify(
            x => x.SendCoreAsync(
                "ReceberAlertaProduto",
                It.IsAny<object[]>(),
                default),
            Times.Once);
    }

    [Fact]
    public async Task VerificarEstoqueBaixo_Should_Send_Alert()
    {
        var produto = CreateProduto();
        produto.Quantity = 2;

        _mockRepository
            .Setup(r => r.GetLowStockAsync(5))
            .ReturnsAsync(new List<Produto> { produto });

        await _service.VerificarEstoqueBaixoAsync(5);

        _mockClientProxy.Verify(
            x => x.SendCoreAsync(
                "ReceberAlertaProduto",
                It.IsAny<object[]>(),
                default),
            Times.Once);
    }

    [Fact]
    public async Task VerificarEstoqueBaixo_Should_Not_Send_When_Empty()
    {
        _mockRepository
            .Setup(r => r.GetLowStockAsync(5))
            .ReturnsAsync(new List<Produto>());

        await _service.VerificarEstoqueBaixoAsync(5);

        _mockClientProxy.Verify(
            x => x.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                default),
            Times.Never);
    }

    [Fact]
    public async Task VerificarEstoqueBaixo_Should_Send_For_Each_Product()
    {
        var p1 = CreateProduto();
        var p2 = CreateProduto();

        p1.Quantity = 2;
        p2.Quantity = 1;

        _mockRepository
            .Setup(r => r.GetLowStockAsync(5))
            .ReturnsAsync(new List<Produto> { p1, p2 });

        await _service.VerificarEstoqueBaixoAsync(5);

        _mockClientProxy.Verify(
            x => x.SendCoreAsync(
                "ReceberAlertaProduto",
                It.IsAny<object[]>(),
                default),
            Times.Exactly(2));
    }
}