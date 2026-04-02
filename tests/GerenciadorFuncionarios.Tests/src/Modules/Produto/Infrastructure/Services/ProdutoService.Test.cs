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
    private readonly Mock<IHubClients> _mockClients;
    private readonly Mock<IClientProxy> _mockClientProxy;

    private readonly ProdutoService _prodService;

    public ProdutoServiceTests()
    {
        _mockLogger = new Mock<ILogger<ProdutoService>>();
        _mockProdRepository = new Mock<IProdutoRepository>();

        _mockHub = new Mock<IHubContext<EstoqueHub>>();
        _mockClients = new Mock<IHubClients>();
        _mockClientProxy = new Mock<IClientProxy>();

        _mockClients
            .Setup(c => c.All)
            .Returns(_mockClientProxy.Object);

        _mockHub
            .Setup(h => h.Clients)
            .Returns(_mockClients.Object);

        _prodService = new ProdutoService(
            _mockProdRepository.Object,
            _mockHub.Object,
            _mockLogger.Object
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

    private UpdateProdutoDTO Create_UpdateDto()
    {
        return new UpdateProdutoDTO
        {
            Name = "Gas 14kg",
            Price = 100
        };
    }

    private UpdateEstoqueDTO Create_UpdateEstoque(int quantity)
    {
        return new UpdateEstoqueDTO
        {
            Quantity = quantity
        };
    }

    private PaginationResponse<ResponseProdutoDTO> Create_Paginated()
    {
        return new PaginationResponse<ResponseProdutoDTO>
        (
            new List<ResponseProdutoDTO>
            {
                new ResponseProdutoDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "Gas 12kg",
                    Type = TipoProduto.GAS,
                    Quantity = 50,
                    Price = 110,
                    IsActive = true
                },
                new ResponseProdutoDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "Gas 12kg",
                    Type = TipoProduto.GAS,
                    Quantity = 50,
                    Price = 110,
                    IsActive = true
                }
            },
            1,
            10,
            2
        );
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
        var dto = Create_RegisterDto();

        Produto? captured = null;

        _mockProdRepository
            .Setup(r => r.Add(It.IsAny<Produto>()))
            .Callback<Produto>(p => captured = p);

        await _prodService.RegistrarProdutoAsync(dto);

        Assert.NotNull(captured);
        Assert.Equal(dto.Name, captured!.Name);
        Assert.Equal(dto.Price, captured.Price);
        Assert.Equal(dto.Quantity, captured.Quantity);
        Assert.Equal(dto.Type, captured.Type);
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
    public async Task RegistrarProdutoAsync_Should_Throw_When_Price_Is_Zero()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas 12kg",
            Type = TipoProduto.GAS,
            Quantity = 50,
            Price = 0
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _prodService.RegistrarProdutoAsync(dto));

        Assert.Equal("Preço deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public async Task RegistrarProdutoAsync_Should_Throw_When_Price_Is_Negative()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas 12kg",
            Type = TipoProduto.GAS,
            Quantity = 50,
            Price = -10
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _prodService.RegistrarProdutoAsync(dto));

        Assert.Equal("Preço deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public async Task RegistrarProdutoAsync_Should_Set_IsActive_True_By_Default()
    {
        var dto = Create_RegisterDto();

        var result = await _prodService.RegistrarProdutoAsync(dto);

        Assert.True(result.Data!.IsActive);
    }

    [Fact]
    public async Task RegistrarProdutoAsync_Should_Set_Initial_Quantity()
    {
        var dto = Create_RegisterDto();

        var result = await _prodService.RegistrarProdutoAsync(dto);

        Assert.Equal(result.Data!.Quantity, dto.Quantity);
    }

    [Fact]
    public async Task ObterProdutoPorId_Should_Return_Mapped_DTO()
    {
        var prod = Create_Produto();
        var id = prod.Id;

        _mockProdRepository.Setup(p => p.GetByIdAsync(id)).ReturnsAsync(prod);

        var result = await _prodService.ObterProdutoPorId(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Data!.Id);
        Assert.Equal(prod.Name, result.Data!.Name);
        Assert.Equal(prod.Price, result.Data!.Price);
        Assert.Equal(prod.Quantity, result.Data!.Quantity);
    }

    [Fact]
    public async Task Atualizar_Should_Update_Name()
    {
        var dto = Create_UpdateDto();
        var prod = Create_Produto();

        _mockProdRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        var result = await _prodService.Atualizar(prod.Id, dto);

        Assert.Equal(result.Data!.Name, dto.Name);
        _mockProdRepository.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Atualizar_Should_Update_Price()
    {
        var dto = Create_UpdateDto();
        var prod = Create_Produto();

        _mockProdRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        var result = await _prodService.Atualizar(prod.Id, dto);

        Assert.Equal(result.Data!.Price, dto.Price);
        _mockProdRepository.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Atualizar_Should_Not_Change_Quantity()
    {
        var dto = Create_UpdateDto();
        var prod = Create_Produto();

        _mockProdRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        var result = await _prodService.Atualizar(prod.Id, dto);

        Assert.Equal(result.Data!.Quantity, prod.Quantity);
    }

    [Fact]
    public async Task Atualizar_Should_Throw_When_Product_Not_Found()
    {
        var dto = Create_UpdateDto();

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(() => _prodService.Atualizar(Guid.NewGuid(), dto));

        Assert.Equal("Produto não encontrado.", ex.Message);
    }

    [Fact]
    public async Task EntradaEstoque_Should_Increase_Quantity()
    {
        var prod = Create_Produto();
        var dto = Create_UpdateEstoque(10);

        _mockProdRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        var oldQuantity = prod.Quantity;

        var result = await _prodService.EntradaEstoque(prod.Id, dto);

        Assert.Equal(oldQuantity + 10, result.Data!.Quantity);
        _mockProdRepository.Verify(p => p.SaveChangesAsync());
    }

    [Fact]
    public async Task EntradaEstoque_Should_Call_Repository_Save()
    {
        var prod = Create_Produto();
        var dto = Create_UpdateEstoque(10);

        _mockProdRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        await _prodService.EntradaEstoque(prod.Id, dto);

        _mockProdRepository.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task EntradaEstoque_Should_Throw_When_Quantity_Is_Zero()
    {
        var dto = Create_UpdateEstoque(0);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _prodService.EntradaEstoque(Guid.NewGuid(), dto));

        Assert.Equal("Quantidade deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public async Task EntradaEstoque_Should_Throw_When_Quantity_Is_Negative()
    {
        var dto = Create_UpdateEstoque(-10);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _prodService.EntradaEstoque(Guid.NewGuid(), dto));

        Assert.Equal("Quantidade deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public async Task EntradaEstoque_Should_Throw_When_Product_Not_Found()
    {
        var dto = Create_UpdateEstoque(10);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _prodService.EntradaEstoque(Guid.NewGuid(), dto));

        Assert.Equal("Produto não encontrado.", ex.Message);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Decrease_Quantity()
    {
        var prod = Create_Produto();
        var dto = Create_UpdateEstoque(10);

        _mockProdRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        var oldQuantity = prod.Quantity;

        var result = await _prodService.BaixarEstoque(prod.Id, dto);

        Assert.Equal(oldQuantity - 10, result.Data!.Quantity);
        _mockProdRepository.Verify(p => p.SaveChangesAsync());
    }

    [Fact]
    public async Task BaixarEstoque_Should_Throw_When_Stock_Is_Insufficient()
    {
        var prod = Create_Produto();
        prod.Quantity = 5;

        var dto = Create_UpdateEstoque(10);

        _mockProdRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _prodService.BaixarEstoque(prod.Id, dto));

        Assert.Equal("Estoque Insulficiente.", ex.Message);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Throw_When_Quantity_Is_Zero()
    {
        var dto = Create_UpdateEstoque(0);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _prodService.BaixarEstoque(Guid.NewGuid(), dto));

        Assert.Equal("Quantidade deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Throw_When_Quantity_Is_Negative()
    {
        var dto = Create_UpdateEstoque(-10);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _prodService.BaixarEstoque(Guid.NewGuid(), dto));

        Assert.Equal("Quantidade deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Throw_When_Product_Not_Found()
    {
        var dto = Create_UpdateEstoque(10);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _prodService.BaixarEstoque(Guid.NewGuid(), dto));

        Assert.Equal("Produto não encontrado.", ex.Message);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Call_SaveChanges()
    {
        var prod = Create_Produto();
        var dto = Create_UpdateEstoque(10);

        _mockProdRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        await _prodService.BaixarEstoque(prod.Id, dto);

        _mockProdRepository.Verify(p => p.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task InativarPorId_Should_Set_IsActive_False()
    {
        var prod = Create_Produto();

        _mockProdRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        await _prodService.InativarPorId(prod.Id);

        Assert.False(prod.IsActive);
        _mockProdRepository.Verify(p => p.SaveChangesAsync());
    }

    [Fact]
    public async Task InativarPorId_Should_Throw_When_Product_Has_Stock()
    {
        var prod = Create_Produto();
        prod.Quantity = 10;

        _mockProdRepository.Setup(p => p.GetByIdAsync(prod.Id)).ReturnsAsync(prod);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _prodService.InativarPorId(prod.Id));

        Assert.Equal("Produto ainda tem estoque, não foi possível inativá-lo.", ex.Message);
    }

    [Fact]
    public async Task VerificarEstoqueBaixoAsync_Should_Send_SignalR_Alert()
    {
        var prod = Create_Produto();
        prod.Quantity = 2;

        _mockProdRepository
            .Setup(r => r.GetLowStockAsync(5))
            .ReturnsAsync(new List<Produto> { prod });

        await _prodService.VerificarEstoqueBaixoAsync(5);

        _mockClientProxy.Verify(
            x => x.SendCoreAsync(
                "ReceberAlertaProduto",
                It.Is<object[]>(o =>
                    o.Length == 1 &&
                    o[0] is ProdutoAlertaDTO),
                default),
            Times.Once);
    }

    [Fact]
    public async Task VerificarEstoqueBaixoAsync_Should_Not_Send_When_No_Product_Below_Limit()
    {
        _mockProdRepository
            .Setup(r => r.GetLowStockAsync(5))
            .ReturnsAsync(new List<Produto>());

        await _prodService.VerificarEstoqueBaixoAsync(5);

        _mockClientProxy.Verify(
            x => x.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                default),
            Times.Never);
    }

    [Fact]
    public async Task VerificarEstoqueBaixoAsync_Should_Send_For_Each_Product()
    {
        var prod1 = Create_Produto();
        var prod2 = Create_Produto();

        prod1.Quantity = 2;
        prod2.Quantity = 1;

        _mockProdRepository
            .Setup(r => r.GetLowStockAsync(5))
            .ReturnsAsync(new List<Produto> { prod1, prod2 });

        await _prodService.VerificarEstoqueBaixoAsync(5);

        _mockClientProxy.Verify(
            x => x.SendCoreAsync(
                "ReceberAlertaProduto",
                It.IsAny<object[]>(),
                default),
            Times.Exactly(2));
    }

    [Fact]
    public async Task SaidaEstoque_Should_Notify_Hub()
    {
        var prod = Create_Produto();
        prod.Quantity = 3;

        _mockProdRepository
            .Setup(p => p.GetByIdAsync(prod.Id))
            .ReturnsAsync(prod);

        _mockProdRepository
            .Setup(p => p.GetLowStockAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Produto> { prod });

        await _prodService.BaixarEstoque(prod.Id, Create_UpdateEstoque(1));

        _mockClientProxy.Verify(
            x => x.SendCoreAsync(
                "ReceberAlertaProduto",
                It.IsAny<object[]>(),
                default),
            Times.Once);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Return_Paginated_Result()
    {
        var paginated = Create_Paginated();

        _mockProdRepository
            .Setup(r => r.GetAllAsync(1, 10, null, null, null, null))
            .ReturnsAsync(paginated);

        var result = await _prodService.ObterTodosProdutos(1, 10, null, null, null, null);

        Assert.True(result.Success);
        Assert.Equal(1, result.Data!.Page);
        Assert.Equal(10, result.Data.PageSize);
        Assert.Equal(paginated.TotalItems, result.Data.TotalItems);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Call_Repository_With_Filters()
    {
        await _prodService.ObterTodosProdutos(
            1,
            10,
            "Gas",
            100,
            200,
            TipoProduto.GAS);

        _mockProdRepository.Verify(r =>
            r.GetAllAsync(
                1,
                10,
                "Gas",
                100,
                200,
                TipoProduto.GAS),
            Times.Once);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Filter_By_Name()
    {
        var paginated = Create_Paginated();

        _mockProdRepository
            .Setup(r => r.GetAllAsync(1, 10, "Gas", null, null, null))
            .ReturnsAsync(paginated);

        var result = await _prodService.ObterTodosProdutos(1, 10, "Gas", null, null, null);

        Assert.All(result.Data!.Items,
            p => Assert.Contains("Gas", p.Name));
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Filter_By_Tipo()
    {
        var paginated = Create_Paginated();

        _mockProdRepository
            .Setup(r => r.GetAllAsync(1, 10, null, null, null, TipoProduto.GAS))
            .ReturnsAsync(paginated);

        var result = await _prodService.ObterTodosProdutos(
            1, 10, null, null, null, TipoProduto.GAS);

        Assert.All(result.Data!.Items,
            p => Assert.Equal(TipoProduto.GAS, p.Type));
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Filter_By_Price_Range()
    {
        var paginated = Create_Paginated();

        _mockProdRepository
            .Setup(r => r.GetAllAsync(1, 10, null, 100, 200, null))
            .ReturnsAsync(paginated);

        var result = await _prodService.ObterTodosProdutos(
            1, 10, null, 100, 200, null);

        Assert.All(result.Data!.Items,
            p => Assert.True(p.Price >= 100 && p.Price <= 200));
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Return_Empty()
    {
        var empty = new PaginationResponse<ResponseProdutoDTO>(
            new List<ResponseProdutoDTO>(),
            1,
            10,
            0);

        _mockProdRepository
            .Setup(r => r.GetAllAsync(1, 10, null, null, null, null))
            .ReturnsAsync(empty);

        var result = await _prodService.ObterTodosProdutos(1, 10, null, null, null, null);

        Assert.Empty(result.Data!.Items);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Maintain_Pagination_With_Filter()
    {
        var paginated = Create_Paginated();

        _mockProdRepository
            .Setup(r => r.GetAllAsync(2, 5, "Gas", null, null, null))
            .ReturnsAsync(paginated);

        var result = await _prodService.ObterTodosProdutos(2, 5, "Gas", null, null, null);

        Assert.Equal(2, result.Data!.Page);
        Assert.Equal(5, result.Data.PageSize);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Return_Success_Response()
    {
        var paginated = Create_Paginated();

        _mockProdRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<TipoProduto?>()))
            .ReturnsAsync(paginated);

        var result = await _prodService.ObterTodosProdutos(1, 10, null, null, null, null);

        Assert.True(result.Success);
    }
}