using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Produto.Application.Services;
using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

public class EntradaEstoqueUseCaseTests
{
    private readonly Mock<IEstoqueService> _mockEstoqueService;

    private readonly EntradaEstoque _useCase;

    public EntradaEstoqueUseCaseTests()
    {
        _mockEstoqueService = new Mock<IEstoqueService>();

        _useCase = new EntradaEstoque(_mockEstoqueService.Object);
    }

    private UpdateEstoqueDTO Create_UpdateEstoque(int quantity)
    {
        return new UpdateEstoqueDTO
        {
            Quantity = quantity
        };
    }

    [Fact]
    public async Task Execute_Should_Call_EstoqueService()
    {
        var id = Guid.NewGuid();
        var dto = Create_UpdateEstoque(10);

        await _useCase.Execute(id, dto);

        _mockEstoqueService.Verify(
            x => x.EntradaEstoque(id, dto.Quantity),
            Times.Once);
    }

    [Fact]
    public async Task Execute_Should_Return_Response_From_Service()
    {
        var id = Guid.NewGuid();
        var dto = Create_UpdateEstoque(10);

        var produto = new Produto
        {
            Id = id,
            Name = "Gas 12kg",
            Quantity = 60,
            Price = 110,
            Type = TipoProduto.GAS,
            IsActive = true
        };

        _mockEstoqueService
            .Setup(x => x.EntradaEstoque(id, dto.Quantity))
            .ReturnsAsync(produto);

        var result = await _useCase.Execute(id, dto);

        Assert.True(result.Success);
        Assert.Equal(produto.Quantity, result.Data!.Quantity);
        Assert.Equal(produto.Id, result.Data.Id);
    }
}
