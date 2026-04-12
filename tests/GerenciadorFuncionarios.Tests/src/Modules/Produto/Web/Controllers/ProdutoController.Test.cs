using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Produto.Application.UseCases;

public class ProdutoControllerTests
{
    private readonly Mock<IRegistrarProduto> _mockRegistrarProduto;
    private readonly Mock<IObterProdutoPorId> _mockObterProdutoPorId;
    private readonly Mock<IInativarProduto> _mockInativarProduto;
    private readonly Mock<IAtualizarProduto> _mockAtualizarProduto;
    private readonly Mock<IEntradaEstoque> _mockEntradaEstoque;
    private readonly Mock<IBaixarEstoque> _mockBaixarEstoque;
    private readonly Mock<IObterTodosProdutos> _mockObterTodosProdutos;
    private readonly ProdutoController _controller;

    public ProdutoControllerTests()
    {
        _mockRegistrarProduto = new Mock<IRegistrarProduto>();
        _mockObterProdutoPorId = new Mock<IObterProdutoPorId>();
        _mockInativarProduto = new Mock<IInativarProduto>();
        _mockAtualizarProduto = new Mock<IAtualizarProduto>();
        _mockEntradaEstoque = new Mock<IEntradaEstoque>();
        _mockBaixarEstoque = new Mock<IBaixarEstoque>();
        _mockObterTodosProdutos = new Mock<IObterTodosProdutos>();
        _controller = new ProdutoController(
            _mockRegistrarProduto.Object,
            _mockObterProdutoPorId.Object,
            _mockInativarProduto.Object,
            _mockAtualizarProduto.Object,
            _mockEntradaEstoque.Object,
            _mockBaixarEstoque.Object,
            _mockObterTodosProdutos.Object);
    }

    private ResponseProdutoDTO Create_Response()
    {
        return new ResponseProdutoDTO
        {
            Id = Guid.NewGuid(),
            Name = "Gas 12kg",
            Price = 110,
            Quantity = 50,
            Type = TipoProduto.GAS,
            IsActive = true
        };
    }

    private RegisterProdutoDTO Create_Register()
    {
        return new RegisterProdutoDTO
        {
            Name = "Gas 12kg",
            Price = 110,
            Quantity = 50,
            Type = TipoProduto.GAS
        };
    }

    private UpdateProdutoDTO Create_Update()
    {
        return new UpdateProdutoDTO
        {
            Name = "Gas 13kg",
            Price = 120
        };
    }

    private UpdateEstoqueDTO Create_UpdateEstoque()
    {
        return new UpdateEstoqueDTO
        {
            Quantity = 10
        };
    }

    [Fact]
    public async Task RegistrarProduto_Should_Return_Ok()
    {
        var dto = Create_Register();
        var response = ApiResponse<ResponseProdutoDTO>.Ok(Create_Response());

        _mockRegistrarProduto.Setup(s => s.Execute(dto))
            .ReturnsAsync(response);

        var result = await _controller.Resgister(dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task RegistrarProduto_Should_Call_Service()
    {
        var dto = Create_Register();

        await _controller.Resgister(dto);

        _mockRegistrarProduto.Verify(
            s => s.Execute(dto),
            Times.Once);
    }

    [Fact]
    public async Task RegistrarProduto_Should_Return_ProductDTO()
    {
        var dto = Create_Register();
        var response = ApiResponse<ResponseProdutoDTO>.Ok(Create_Response());

        _mockRegistrarProduto.Setup(s => s.Execute(dto))
            .ReturnsAsync(response);

        var result = await _controller.Resgister(dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ApiResponse<ResponseProdutoDTO>>(ok.Value);

        Assert.Equal(dto.Name, value.Data!.Name);
    }

    [Fact]
    public async Task RegistrarProduto_Should_Return_BadRequest_When_Invalid_Data()
    {
        var dto = Create_Register();

        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.Resgister(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RegistrarProduto_Should_Return_Conflict_When_Product_Already_Exists()
    {
        var dto = Create_Register();

        _mockRegistrarProduto
            .Setup(s => s.Execute(dto))
            .ThrowsAsync(new EntityAlreadyExistsException("Produto já existe"));

        var result = await _controller.Resgister(dto);

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task ObterProdutoPorId_Should_Return_Ok()
    {
        var response = ApiResponse<ResponseProdutoDTO>.Ok(Create_Response());

        _mockObterProdutoPorId
            .Setup(s => s.Execute(It.IsAny<Guid>()))
            .ReturnsAsync(response);

        var result = await _controller.GetProdutoById(Guid.NewGuid());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ObterProdutoPorId_Should_Call_Service()
    {
        await _controller.GetProdutoById(Guid.NewGuid());

        _mockObterProdutoPorId.Verify(
            s => s.Execute(It.IsAny<Guid>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterProdutoPorId_Should_Return_NotFound_When_Not_Exists()
    {
        _mockObterProdutoPorId
            .Setup(s => s.Execute(It.IsAny<Guid>()))
            .ThrowsAsync(new EntityNotFoundException("Produto não encontrado"));

        var result = await _controller.GetProdutoById(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Atualizar_Should_Return_Ok()
    {
        var dto = Create_Update();
        var response = ApiResponse<ResponseProdutoDTO>.Ok(Create_Response());

        _mockAtualizarProduto
            .Setup(s => s.Execute(It.IsAny<Guid>(), dto))
            .ReturnsAsync(response);

        var result = await _controller.UpdateProduto(Guid.NewGuid(), dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Atualizar_Should_Call_Service()
    {
        var dto = Create_Update();

        await _controller.UpdateProduto(Guid.NewGuid(), dto);

        _mockAtualizarProduto.Verify(
            s => s.Execute(It.IsAny<Guid>(), dto),
            Times.Once);
    }

    [Fact]
    public async Task Atualizar_Should_Return_NotFound_When_Product_Not_Exists()
    {
        var dto = Create_Update();

        _mockAtualizarProduto
            .Setup(s => s.Execute(It.IsAny<Guid>(), dto))
            .ThrowsAsync(new EntityNotFoundException("Produto não encontrado"));

        var result = await _controller.UpdateProduto(Guid.NewGuid(), dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Atualizar_Should_Return_BadRequest_When_Invalid_Data()
    {
        var dto = new UpdateProdutoDTO();

        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.UpdateProduto(Guid.NewGuid(), dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task EntradaEstoque_Should_Return_Ok()
    {
        var dto = Create_UpdateEstoque();
        var response = ApiResponse<ResponseProdutoDTO>.Ok(Create_Response());

        _mockEntradaEstoque
            .Setup(s => s.Execute(It.IsAny<Guid>(), dto))
            .ReturnsAsync(response);

        var result = await _controller.EntradaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task EntradaEstoque_Should_Call_Service()
    {
        var dto = Create_UpdateEstoque();

        await _controller.EntradaEstoque(Guid.NewGuid(), dto);

        _mockEntradaEstoque.Verify(
            s => s.Execute(It.IsAny<Guid>(), dto),
            Times.Once);
    }

    [Fact]
    public async Task EntradaEstoque_Should_Return_NotFound_When_Product_Not_Exists()
    {
        var dto = Create_UpdateEstoque();

        _mockEntradaEstoque
            .Setup(s => s.Execute(It.IsAny<Guid>(), dto))
            .ThrowsAsync(new EntityNotFoundException("Produto não encontrado"));

        var result = await _controller.EntradaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task EntradaEstoque_Should_Return_BadRequest_When_Invalid_Quantity()
    {
        var dto = Create_UpdateEstoque();

        _mockEntradaEstoque
            .Setup(s => s.Execute(It.IsAny<Guid>(), dto))
            .ThrowsAsync(new InvalidOperationException("Quantidade inválida"));

        var result = await _controller.EntradaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Return_Ok()
    {
        var dto = Create_UpdateEstoque();
        var response = ApiResponse<ResponseProdutoDTO>.Ok(Create_Response());

        _mockBaixarEstoque
            .Setup(s => s.Execute(It.IsAny<Guid>(), dto))
            .ReturnsAsync(response);

        var result = await _controller.SaidaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Call_Service()
    {
        var dto = Create_UpdateEstoque();

        await _controller.SaidaEstoque(Guid.NewGuid(), dto);

        _mockBaixarEstoque.Verify(
            s => s.Execute(It.IsAny<Guid>(), dto),
            Times.Once);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Return_NotFound_When_Product_Not_Exists()
    {
        var dto = Create_UpdateEstoque();

        _mockBaixarEstoque
            .Setup(s => s.Execute(It.IsAny<Guid>(), dto))
            .ThrowsAsync(new EntityNotFoundException("Produto não encontrado"));

        var result = await _controller.SaidaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Return_BadRequest_When_Invalid_Quantity()
    {
        var dto = Create_UpdateEstoque();

        _mockBaixarEstoque
            .Setup(s => s.Execute(It.IsAny<Guid>(), dto))
            .ThrowsAsync(new InvalidOperationException("Quantidade inválida"));

        var result = await _controller.SaidaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Return_BadRequest_When_Stock_Insufficient()
    {
        var dto = Create_UpdateEstoque();

        _mockBaixarEstoque
            .Setup(s => s.Execute(It.IsAny<Guid>(), dto))
            .ThrowsAsync(new InvalidOperationException("Estoque insuficiente"));

        var result = await _controller.SaidaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Inativar_Should_Return_NoContent()
    {
        var result = await _controller.InactiveById(Guid.NewGuid());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Inativar_Should_Call_Service()
    {
        await _controller.InactiveById(Guid.NewGuid());

        _mockInativarProduto.Verify(
            s => s.Execute(It.IsAny<Guid>()),
            Times.Once);
    }

    [Fact]
    public async Task Inativar_Should_Return_NotFound()
    {
        _mockInativarProduto
            .Setup(s => s.Execute(It.IsAny<Guid>()))
            .ThrowsAsync(new EntityNotFoundException("Produto não encontrado"));

        var result = await _controller.InactiveById(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(result);
    }
    [Fact]
    public async Task ObterTodosProdutos_Should_Return_Ok()
    {
        var paginated = new PaginationResponse<ResponseProdutoDTO>(
            new List<ResponseProdutoDTO> { Create_Response() },
            1, 10, 1);

        var response =
            ApiResponse<PaginationResponse<ResponseProdutoDTO>>
            .Ok(paginated);

        _mockObterTodosProdutos
            .Setup(s => s.Execute(
                1, 10, null, null, null, null))
            .ReturnsAsync(response);

        var result = await _controller.GetAllProdutos(
            1, 10, null, null, null, null);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Call_Service()
    {
        await _controller.GetAllProdutos(
            1, 10, "Gas", 100, 200, TipoProduto.GAS);

        _mockObterTodosProdutos.Verify(s =>
            s.Execute(
                1, 10, "Gas", 100, 200, TipoProduto.GAS),
            Times.Once);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Return_Paginated()
    {
        var paginated = new PaginationResponse<ResponseProdutoDTO>(
            new List<ResponseProdutoDTO> { Create_Response() },
            2, 5, 10);

        var response =
            ApiResponse<PaginationResponse<ResponseProdutoDTO>>
            .Ok(paginated);

        _mockObterTodosProdutos
            .Setup(s => s.Execute(
                2, 5, null, null, null, null))
            .ReturnsAsync(response);

        var result = await _controller.GetAllProdutos(
            2, 5, null, null, null, null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ApiResponse<PaginationResponse<ResponseProdutoDTO>>>(ok.Value);

        Assert.Equal(2, value.Data!.Page);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Filter_By_Name()
    {
        await _controller.GetAllProdutos(1, 10, "Gas", null, null, null);

        _mockObterTodosProdutos.Verify(s =>
            s.Execute(
                1, 10, "Gas", null, null, null),
            Times.Once);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Filter_By_Type()
    {
        await _controller.GetAllProdutos(
            1, 10, null, null, null, TipoProduto.GAS);

        _mockObterTodosProdutos.Verify(s =>
            s.Execute(
                1, 10, null, null, null, TipoProduto.GAS),
            Times.Once);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Filter_By_Price()
    {
        await _controller.GetAllProdutos(
            1, 10, null, 100, 200, null);

        _mockObterTodosProdutos.Verify(s =>
            s.Execute(
                1, 10, null, 100, 200, null),
            Times.Once);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Return_Empty()
    {
        var paginated = new PaginationResponse<ResponseProdutoDTO>(
            new List<ResponseProdutoDTO>(),
            1, 10, 0);

        var response =
            ApiResponse<PaginationResponse<ResponseProdutoDTO>>
            .Ok(paginated);

        _mockObterTodosProdutos
            .Setup(s => s.Execute(
                1, 10, null, null, null, null))
            .ReturnsAsync(response);

        var result = await _controller.GetAllProdutos(
            1, 10, null, null, null, null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ApiResponse<PaginationResponse<ResponseProdutoDTO>>>(ok.Value);

        Assert.Empty(value.Data!.Items);
    }
}