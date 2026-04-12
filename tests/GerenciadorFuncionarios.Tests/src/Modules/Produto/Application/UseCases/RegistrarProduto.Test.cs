using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Produto.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Produto.Domain.Models;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

public class RegistrarProdutoUseCaseTests
{
    private readonly Mock<IProdutoRepository> _mockRepository;

    private readonly RegistrarProduto _useCase;

    public RegistrarProdutoUseCaseTests()
    {
        _mockRepository = new Mock<IProdutoRepository>();

        _useCase = new RegistrarProduto(_mockRepository.Object);
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

    [Fact]
    public async Task Execute_Should_Create_Product_When_Data_Is_Valid()
    {
        var dto = Create_RegisterDto();

        var result = await _useCase.Execute(dto);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_Name_Already_Exists()
    {
        var dto = Create_RegisterDto();

        _mockRepository.Setup(p => p.AnyByNameAsync(It.IsAny<string>())).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _useCase.Execute(dto));

        Assert.Equal($"O produto {dto.Name} já existe.", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_Return_ApiResponse_With_Product()
    {
        var dto = Create_RegisterDto();

        var result = await _useCase.Execute(dto);

        Assert.IsType<ApiResponse<ResponseProdutoDTO>>(result);
        Assert.True(result.Success);
        Assert.Equal(result.Data!.Name, dto.Name);
        Assert.Equal(result.Data.Price, dto.Price);
        Assert.Equal(result.Data.Quantity, dto.Quantity);
    }

    [Fact]
    public async Task Execute_Should_Call_Repository_Add()
    {
        var dto = Create_RegisterDto();

        await _useCase.Execute(dto);

        _mockRepository.Verify(p => p.Add(It.IsAny<Produto>()), Times.Once);
    }

    [Fact]
    public async Task Execute_Should_Map_DTO_To_Entity()
    {
        var dto = Create_RegisterDto();

        Produto? captured = null;

        _mockRepository
            .Setup(r => r.Add(It.IsAny<Produto>()))
            .Callback<Produto>(p => captured = p);

        await _useCase.Execute(dto);

        Assert.NotNull(captured);
        Assert.Equal(dto.Name, captured!.Name);
        Assert.Equal(dto.Price, captured.Price);
        Assert.Equal(dto.Quantity, captured.Quantity);
        Assert.Equal(dto.Type, captured.Type);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_Price_Is_Zero()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas 12kg",
            Type = TipoProduto.GAS,
            Quantity = 50,
            Price = 0
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.Execute(dto));

        Assert.Equal("Preço deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_Throw_When_Price_Is_Negative()
    {
        var dto = new RegisterProdutoDTO
        {
            Name = "Gas 12kg",
            Type = TipoProduto.GAS,
            Quantity = 50,
            Price = -10
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.Execute(dto));

        Assert.Equal("Preço deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public async Task Execute_Should_Set_IsActive_True_By_Default()
    {
        var dto = Create_RegisterDto();

        var result = await _useCase.Execute(dto);

        Assert.True(result.Data!.IsActive);
    }

    [Fact]
    public async Task Execute_Should_Set_Initial_Quantity()
    {
        var dto = Create_RegisterDto();

        var result = await _useCase.Execute(dto);

        Assert.Equal(dto.Quantity, result.Data!.Quantity);
    }
}
