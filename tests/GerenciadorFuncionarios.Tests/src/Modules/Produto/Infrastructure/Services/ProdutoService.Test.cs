using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using GerenciadorFuncionarios.Hubs;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Infrastructure.Services;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _mockProdRepository;
    private readonly Mock<ILogger<ProdutoService>> _mockLogger;
    private readonly Mock<IHubContext<EstoqueHub>> _mockHub;

    private readonly ProdutoService _prodService;

    public ProdutoServiceTests()
    {
        _mockLogger = new Mock<ILogger<ProdutoService>>();
        _mockProdRepository = new Mock<IProdutoRepository>();
        _mockHub = new Mock<IHubContext<EstoqueHub>>();

        _prodService = new ProdutoService(
            _mockProdRepository.Object,
            _mockHub.Object,
            _mockLogger!.Object
        );
    }

    private RegisterProdutoDTO Create_RegisterDto()
    {
        return new RegisterProdutoDTO
        {
            Name = "Gas 12kg",
            Type = TipoProduto.GAS,
            Quantity = 50,
            Price = 110
        };
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
    public async Task RegistrarProdutoAsync_Should_Create_Product_When_Data_Is_Valid()
    {
        var dto = Create_RegisterDto();

        var result = await _prodService.RegistrarProdutoAsync(dto);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task RegistrarProdutoAsync_Should_Throw_When_Name_Already_Exists()
    {
        var dto = Create_RegisterDto();

        _mockProdRepository.Setup(p => p.AnyByNameAsync(It.IsAny<string>())).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _prodService.RegistrarProdutoAsync(dto));

        Assert.Equal($"O produto {dto.Name} já existe.", ex.Message);
    }

    [Fact]
    public async Task RegistrarProdutoAsync_Should_Return_ApiResponse_With_Product()
    {
        var dto = Create_RegisterDto();

        var result = await _prodService.RegistrarProdutoAsync(dto);

        Assert.IsType<ApiResponse<ResponseProdutoDTO>>(result.Data);
        Assert.True(result.Success);
        Assert.Equal(result.Data!.Name, dto.Name);
        Assert.Equal(result.Data.Price, dto.Price);
        Assert.Equal(result.Data.Quantity, dto.Quantity);
    }

    [Fact]
    public async Task RegistrarProdutoAsync_Should_Call_Repository_Add()
    {
        var dto = Create_RegisterDto();

        _mockProdRepository.Setup(p => p.Add(It.IsAny<Produto>()));

        var result = await _prodService.RegistrarProdutoAsync(dto);

        _mockProdRepository.Verify(p => p.Add(It.IsAny<Produto>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarProdutoAsync_Should_Map_DTO_To_Entity()
    {

    }

    [Fact]
    public async Task ObterProdutoPorId_Should_Return_Product_When_Exists()
    {
        var prod = Create_Produto();
        var id = prod.Id;

        _mockProdRepository.Setup(p => p.GetByIdAsync(id)).ReturnsAsync(prod);

        var result = await _prodService.ObterProdutoPorId(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Data!.Id);
    }

    [Fact]
    public async Task ObterProdutoPorId_Should_Throw_When_Not_Found()
    {
        _mockProdRepository.Setup(p => p.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _prodService.ObterProdutoPorId(Guid.NewGuid()));

        Assert.Equal("Produto não encontrado.", ex.Message);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Return_ApiResponse()
    {
        var prod = Create_Produto();
        var id = prod.Id;

        _mockProdRepository.Setup(p => p.GetByIdAsync(id)).ReturnsAsync(prod);

        var result = await _prodService.ObterProdutoPorId(id);

        Assert.IsType<ApiResponse<ResponseProdutoDTO>>(result.Data);
        Assert.True(result.Success);
        Assert.Equal(id, result.Data!.Id);
        Assert.Equal(result.Data!.Name, prod.Name);
        Assert.Equal(result.Data.Price, prod.Price);
        Assert.Equal(result.Data.Quantity, prod.Quantity);
    }

    [Fact]
    public async Task InativarPorId_Should_Inactivate_Product_When_Quantity_Zero()
    {
        var prod = Create_Produto();
        prod.Quantity = 0;
        var id = prod.Id;

        _mockProdRepository
            .Setup(p => p.GetByIdAsync(id))
            .ReturnsAsync(prod);

        await _prodService.InativarPorId(id);

        Assert.False(prod.IsActive);

        _mockProdRepository.Verify(
            p => p.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task InativarPorId_Should_Throw_When_Product_Not_Found()
    {
        _mockProdRepository.Setup(p => p.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _prodService.InativarPorId(Guid.NewGuid()));

        Assert.Equal("Produto não encontrado.", ex.Message);
    }

    [Fact]
    public async Task InativarPorId_Should_Throw_When_Product_Has_Stock()
    {
        _mockProdRepository.Setup(p => p.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _prodService.InativarPorId(Guid.NewGuid()));

        Assert.Equal("Produto não pode ser inativado pois ainda tem estoque.", ex.Message);
    }
}