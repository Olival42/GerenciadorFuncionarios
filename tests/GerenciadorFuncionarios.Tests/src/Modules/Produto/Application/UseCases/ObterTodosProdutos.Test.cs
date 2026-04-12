using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

public class ObterTodosProdutosUseCaseTests
{
    private readonly Mock<IProdutoRepository> _mockRepository;

    private readonly ObterTodosProdutos _useCase;

    public ObterTodosProdutosUseCaseTests()
    {
        _mockRepository = new Mock<IProdutoRepository>();

        _useCase = new ObterTodosProdutos(_mockRepository.Object);
    }

    private PaginationResponse<ResponseProdutoDTO> Create_Paginated()
    {
        return new PaginationResponse<ResponseProdutoDTO>(
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
                    Name = "Gas 13kg",
                    Type = TipoProduto.GAS,
                    Quantity = 30,
                    Price = 120,
                    IsActive = true
                }
            },
            1,
            10,
            2
        );
    }

    [Fact]
    public async Task Execute_Should_Return_Paginated_Result()
    {
        var paginated = Create_Paginated();

        _mockRepository
            .Setup(r => r.GetAllAsync(1, 10, null, null, null, null))
            .ReturnsAsync(paginated);

        var result = await _useCase.Execute(1, 10, null, null, null, null);

        Assert.True(result.Success);
        Assert.Equal(1, result.Data!.Page);
        Assert.Equal(10, result.Data.PageSize);
        Assert.Equal(paginated.TotalItems, result.Data.TotalItems);
    }

    [Fact]
    public async Task Execute_Should_Call_Repository_With_Filters()
    {
        await _useCase.Execute(
            1,
            10,
            "Gas",
            100,
            200,
            TipoProduto.GAS);

        _mockRepository.Verify(r =>
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
    public async Task Execute_Should_Filter_By_Name()
    {
        var paginated = Create_Paginated();

        _mockRepository
            .Setup(r => r.GetAllAsync(1, 10, "Gas", null, null, null))
            .ReturnsAsync(paginated);

        var result = await _useCase.Execute(1, 10, "Gas", null, null, null);

        Assert.All(result.Data!.Items,
            p => Assert.Contains("Gas", p.Name));
    }

    [Fact]
    public async Task Execute_Should_Filter_By_Tipo()
    {
        var paginated = Create_Paginated();

        _mockRepository
            .Setup(r => r.GetAllAsync(1, 10, null, null, null, TipoProduto.GAS))
            .ReturnsAsync(paginated);

        var result = await _useCase.Execute(
            1, 10, null, null, null, TipoProduto.GAS);

        Assert.All(result.Data!.Items,
            p => Assert.Equal(TipoProduto.GAS, p.Type));
    }

    [Fact]
    public async Task Execute_Should_Filter_By_Price_Range()
    {
        var paginated = Create_Paginated();

        _mockRepository
            .Setup(r => r.GetAllAsync(1, 10, null, 100, 200, null))
            .ReturnsAsync(paginated);

        var result = await _useCase.Execute(
            1, 10, null, 100, 200, null);

        Assert.All(result.Data!.Items,
            p => Assert.True(p.Price >= 100 && p.Price <= 200));
    }

    [Fact]
    public async Task Execute_Should_Return_Empty()
    {
        var empty = new PaginationResponse<ResponseProdutoDTO>(
            new List<ResponseProdutoDTO>(),
            1,
            10,
            0);

        _mockRepository
            .Setup(r => r.GetAllAsync(1, 10, null, null, null, null))
            .ReturnsAsync(empty);

        var result = await _useCase.Execute(1, 10, null, null, null, null);

        Assert.Empty(result.Data!.Items);
    }

    [Fact]
    public async Task Execute_Should_Maintain_Pagination_With_Filter()
    {
        var paginated = Create_Paginated();

        _mockRepository
            .Setup(r => r.GetAllAsync(2, 5, "Gas", null, null, null))
            .ReturnsAsync(paginated);

        var result = await _useCase.Execute(2, 5, "Gas", null, null, null);

        Assert.Equal(2, result.Data!.Page);
        Assert.Equal(5, result.Data.PageSize);
    }

    [Fact]
    public async Task Execute_Should_Return_Success_Response()
    {
        var paginated = Create_Paginated();

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<TipoProduto?>()))
            .ReturnsAsync(paginated);

        var result = await _useCase.Execute(1, 10, null, null, null, null);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Execute_Should_Map_DTOs_To_Data()
    {
        var paginated = Create_Paginated();
        _mockRepository.Setup(r => r.GetAllAsync(1, 10, null, null, null, null)).ReturnsAsync(paginated);

        var result = await _useCase.Execute(1, 10, null, null, null, null);

        Assert.Equal(
            paginated.Items.Select(x => x.Id).OrderBy(x => x),
            result.Data!.Items.Select(x => x.Id).OrderBy(x => x));
    }
}
