using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers;
using GerenciadorFuncionarios.Modules.Produto.Application.Services;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Produto.Domain.Enums;
using GerenciadorFuncionarios.Domain.Exceptions;

public class ProdutoControllerTests
{
    private readonly Mock<IProdutoService> _mockService;
    private readonly ProdutoController _controller;

    public ProdutoControllerTests()
    {
        _mockService = new Mock<IProdutoService>();
        _controller = new ProdutoController(_mockService.Object);
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

        _mockService.Setup(s => s.RegistrarProdutoAsync(dto))
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

        _mockService.Verify(
            s => s.RegistrarProdutoAsync(dto),
            Times.Once);
    }

    [Fact]
    public async Task RegistrarProduto_Should_Return_ProductDTO()
    {
        var dto = Create_Register();
        var response = ApiResponse<ResponseProdutoDTO>.Ok(Create_Response());

        _mockService.Setup(s => s.RegistrarProdutoAsync(dto))
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

        _mockService
            .Setup(s => s.RegistrarProdutoAsync(dto))
            .ThrowsAsync(new EntityAlreadyExistsException("Produto já existe"));

        var result = await _controller.Resgister(dto);

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task ObterProdutoPorId_Should_Return_Ok()
    {
        var response = ApiResponse<ResponseProdutoDTO>.Ok(Create_Response());

        _mockService
            .Setup(s => s.ObterProdutoPorId(It.IsAny<Guid>()))
            .ReturnsAsync(response);

        var result = await _controller.GetProdutoById(Guid.NewGuid());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ObterProdutoPorId_Should_Call_Service()
    {
        await _controller.GetProdutoById(Guid.NewGuid());

        _mockService.Verify(
            s => s.ObterProdutoPorId(It.IsAny<Guid>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterProdutoPorId_Should_Return_NotFound_When_Not_Exists()
    {
        _mockService
            .Setup(s => s.ObterProdutoPorId(It.IsAny<Guid>()))
            .ThrowsAsync(new EntityNotFoundException("Produto não encontrado"));

        var result = await _controller.GetProdutoById(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Atualizar_Should_Return_Ok()
    {
        var dto = Create_Update();
        var response = ApiResponse<ResponseProdutoDTO>.Ok(Create_Response());

        _mockService
            .Setup(s => s.Atualizar(It.IsAny<Guid>(), dto))
            .ReturnsAsync(response);

        var result = await _controller.UpdateProduto(Guid.NewGuid(), dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Atualizar_Should_Call_Service()
    {
        var dto = Create_Update();

        await _controller.UpdateProduto(Guid.NewGuid(), dto);

        _mockService.Verify(
            s => s.Atualizar(It.IsAny<Guid>(), dto),
            Times.Once);
    }

    [Fact]
    public async Task Atualizar_Should_Return_NotFound_When_Product_Not_Exists()
    {
        var dto = Create_Update();

        _mockService
            .Setup(s => s.Atualizar(It.IsAny<Guid>(), dto))
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

        _mockService
            .Setup(s => s.EntradaEstoque(It.IsAny<Guid>(), dto))
            .ReturnsAsync(response);

        var result = await _controller.EntradaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task EntradaEstoque_Should_Call_Service()
    {
        var dto = Create_UpdateEstoque();

        await _controller.EntradaEstoque(Guid.NewGuid(), dto);

        _mockService.Verify(
            s => s.EntradaEstoque(It.IsAny<Guid>(), dto),
            Times.Once);
    }

    [Fact]
    public async Task EntradaEstoque_Should_Return_NotFound_When_Product_Not_Exists()
    {
        var dto = Create_UpdateEstoque();

        _mockService
            .Setup(s => s.EntradaEstoque(It.IsAny<Guid>(), dto))
            .ThrowsAsync(new EntityNotFoundException("Produto não encontrado"));

        var result = await _controller.EntradaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task EntradaEstoque_Should_Return_BadRequest_When_Invalid_Quantity()
    {
        var dto = Create_UpdateEstoque();

        _mockService
            .Setup(s => s.EntradaEstoque(It.IsAny<Guid>(), dto))
            .ThrowsAsync(new InvalidOperationException("Quantidade inválida"));

        var result = await _controller.EntradaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Return_Ok()
    {
        var dto = Create_UpdateEstoque();
        var response = ApiResponse<ResponseProdutoDTO>.Ok(Create_Response());

        _mockService
            .Setup(s => s.BaixarEstoque(It.IsAny<Guid>(), dto))
            .ReturnsAsync(response);

        var result = await _controller.SaidaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Call_Service()
    {
        var dto = Create_UpdateEstoque();

        await _controller.SaidaEstoque(Guid.NewGuid(), dto);

        _mockService.Verify(
            s => s.BaixarEstoque(It.IsAny<Guid>(), dto),
            Times.Once);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Return_NotFound_When_Product_Not_Exists()
    {
        var dto = Create_UpdateEstoque();

        _mockService
            .Setup(s => s.BaixarEstoque(It.IsAny<Guid>(), dto))
            .ThrowsAsync(new EntityNotFoundException("Produto não encontrado"));

        var result = await _controller.SaidaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Return_BadRequest_When_Invalid_Quantity()
    {
        var dto = Create_UpdateEstoque();

        _mockService
            .Setup(s => s.BaixarEstoque(It.IsAny<Guid>(), dto))
            .ThrowsAsync(new InvalidOperationException("Quantidade inválida"));

        var result = await _controller.SaidaEstoque(Guid.NewGuid(), dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task BaixarEstoque_Should_Return_BadRequest_When_Stock_Insufficient()
    {
        var dto = Create_UpdateEstoque();

        _mockService
            .Setup(s => s.BaixarEstoque(It.IsAny<Guid>(), dto))
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

        _mockService.Verify(
            s => s.InativarPorId(It.IsAny<Guid>()),
            Times.Once);
    }

    [Fact]
    public async Task Inativar_Should_Return_NotFound()
    {
        _mockService
            .Setup(s => s.InativarPorId(It.IsAny<Guid>()))
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

        _mockService
            .Setup(s => s.ObterTodosProdutos(
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

        _mockService.Verify(s =>
            s.ObterTodosProdutos(
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

        _mockService
            .Setup(s => s.ObterTodosProdutos(
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

        _mockService.Verify(s =>
            s.ObterTodosProdutos(
                1, 10, "Gas", null, null, null),
            Times.Once);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Filter_By_Type()
    {
        await _controller.GetAllProdutos(
            1, 10, null, null, null, TipoProduto.GAS);

        _mockService.Verify(s =>
            s.ObterTodosProdutos(
                1, 10, null, null, null, TipoProduto.GAS),
            Times.Once);
    }

    [Fact]
    public async Task ObterTodosProdutos_Should_Filter_By_Price()
    {
        await _controller.GetAllProdutos(
            1, 10, null, 100, 200, null);

        _mockService.Verify(s =>
            s.ObterTodosProdutos(
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

        _mockService
            .Setup(s => s.ObterTodosProdutos(
                1, 10, null, null, null, null))
            .ReturnsAsync(response);

        var result = await _controller.GetAllProdutos(
            1, 10, null, null, null, null);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ApiResponse<PaginationResponse<ResponseProdutoDTO>>>(ok.Value);

        Assert.Empty(value.Data!.Items);
    }
}