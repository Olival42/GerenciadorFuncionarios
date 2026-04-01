using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Controllers;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Requests;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Domain.Exceptions;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using Microsoft.Extensions.Logging;
using GerenciadorFuncionarios.Modules.Funcionario.Application.Services;
using GerenciadorFuncionarios.Modules.Funcionario.Infrastructure.Services;

public class FuncionarioControllerTests
{
    private readonly Mock<IFuncionarioRepository> _mockRepository;
    private readonly Mock<ILogger<FuncionarioService>> _mockLogger;
    private readonly Mock<IFuncionarioService> _mockService;
    private readonly FuncionarioController _controller;

    public FuncionarioControllerTests()
    {
        _mockRepository = new Mock<IFuncionarioRepository>();
        _mockLogger = new Mock<ILogger<FuncionarioService>>();
        _mockService = new Mock<IFuncionarioService>();
        _controller = new FuncionarioController(_mockService.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    private  RegisterFuncionarioDTO Create_RegisterDto()
    {
        return new RegisterFuncionarioDTO
        {
            Name = "Admin",
            Email = "teste@email.com",
            Password = "123456",
            Role = Role.GERENTE,
            CPF = "12345678900",
            Phone = "44999999999",
        };
    }

    private ResponseFuncionarioDTO Create_ResponseDto()
    {
        return new ResponseFuncionarioDTO
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Email = "teste@email.com",
        };
    }

    [Fact]
    public async Task Register_Should_Return_Created_With_Valid_Data()
    {
        var req = Create_RegisterDto();

        var res = Create_ResponseDto();

        _mockService.Setup(s => s.RegistrarFuncionarioAsync(req))
            .ReturnsAsync(ApiResponse<ResponseFuncionarioDTO>.Ok(res));

        var result = await _controller.Resgister(req);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var apiResponse = Assert.IsType<ApiResponse<ResponseFuncionarioDTO>>(createdResult.Value);

        Assert.Equal(res.Name, apiResponse.Data!.Name);
        Assert.Equal(res.Email, apiResponse.Data.Email);
        Assert.Equal(Role.GERENTE, apiResponse.Data.Role);
    }

    [Fact]
    public async Task Register_Should_Call_Service_RegistrarFuncionarioAsync()
    {
        var req = Create_RegisterDto();

        var res = Create_ResponseDto();

        _mockService.Setup(s => s.RegistrarFuncionarioAsync(req))
            .ReturnsAsync(ApiResponse<ResponseFuncionarioDTO>.Ok(res));

        var result = await _controller.Resgister(req);

        _mockService.Verify(s => s.RegistrarFuncionarioAsync(
            req),
            Times.Once());
    }

    [Fact]
    public async Task GetFuncionarioById_Should_Return_Ok_With_Valid_Id()
    {
        var funcionarioId = Guid.NewGuid();
        var responseDto = Create_ResponseDto();

        _mockService.Setup(s => s.ObterFuncionarioPorId(funcionarioId))
            .ReturnsAsync(ApiResponse<ResponseFuncionarioDTO>.Ok(responseDto));

        var result = await _controller.GetFuncionarioById(funcionarioId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var apiResponse = Assert.IsType<ApiResponse<ResponseFuncionarioDTO>>(okResult.Value);

        Assert.Equal(funcionarioId, apiResponse.Data!.Id);
        Assert.Equal("João Silva", apiResponse.Data.Name);
    }

    [Fact]
    public async Task GetFuncionarioById_Should_Return_NotFound_When_Id_Not_Exist()
    {
        _mockService.Setup(s => s.ObterFuncionarioPorId(It.IsAny<Guid>()))
            .ThrowsAsync(new EntityNotFoundException("Funcionário não encontrado"));

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _controller.GetFuncionarioById(Guid.NewGuid()));

        Assert.Equal("Funcionário não encontrado", ex.Message);
    }

    [Fact]
    public async Task GetFuncionarioById_Should_Call_Service_ObterFuncionarioPorId()
    {
        var funcionarioId = Guid.NewGuid();
        var responseDto = Create_ResponseDto();

        _mockService.Setup(s => s.ObterFuncionarioPorId(funcionarioId))
            .ReturnsAsync(ApiResponse<ResponseFuncionarioDTO>.Ok(responseDto));

        var result = await _controller.GetFuncionarioById(funcionarioId);

        _mockService.Verify(s => s.ObterFuncionarioPorId(
            funcionarioId),
            Times.Once());
    }

    [Fact]
    public async Task InactiveById_Should_Return_NoContent_When_Successful()
    {
        var funcionarioId = Guid.NewGuid();

        _mockService.Setup(s => s.InativarPorId(funcionarioId));

        var result = await _controller.InactiveById(funcionarioId);

        var noContentResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task InactiveById_Should_Call_Service_InativarPorId()
    {
        _mockService.Setup(s => s.InativarPorId(It.IsAny<Guid>()));

        var result = await _controller.InactiveById(Guid.NewGuid());

        _mockService.Verify(s => s.InativarPorId(
            It.IsAny<Guid>()),
            Times.Once());
    }

    [Fact]
    public async Task InactiveById_Should_Return_NotFound_When_Id_Not_Exist()
    {
        _mockService.Setup(s => s.InativarPorId(It.IsAny<Guid>()))
            .ThrowsAsync(new EntityNotFoundException("Funcionário não encontrado"));

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _controller.InactiveById(Guid.NewGuid()));

        Assert.Equal("Funcionário não encontrado", ex.Message);
    }

    [Fact]
    public async Task UpdateFuncionario_Should_Return_Ok_With_Valid_Data()
    {
        var id = Guid.NewGuid();
        var req = new UpdateFuncionarioDTO
        {
            Name = "Admin",
        };

        var res = Create_ResponseDto();

        _mockService.Setup(s => s.Atualizar(id, req))
            .ReturnsAsync(ApiResponse<ResponseFuncionarioDTO>.Ok(res));

        var result = await _controller.UpdateFuncionario(id, req);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var apiResponse = Assert.IsType<ApiResponse<ResponseFuncionarioDTO>>(okResult.Value);

        Assert.Equal(res.Name, apiResponse.Data!.Name);
        Assert.Equal(res.Email, apiResponse.Data.Email);
    }

    [Fact]
    public async Task UpdateFuncionario_Should_Call_Service_Atualizar()
    {
        var id = Guid.NewGuid();
        var req = new UpdateFuncionarioDTO
        {
            Name = "Admin",
        };

        _mockService.Setup(s => s.Atualizar(id, req))
            .ReturnsAsync(It.IsAny<ApiResponse<ResponseFuncionarioDTO>>());

        var result = await _controller.UpdateFuncionario(id, req);

        _mockService.Verify(s => s.Atualizar(
            id,
            req),
            Times.Once());
    }

    [Fact]
    public async Task UpdateFuncionario_Should_Return_NotFound_When_Id_Not_Exist()
    {
        var id = Guid.NewGuid();
        var req = new UpdateFuncionarioDTO { Name = "Admin" };

        _mockService.Setup(s => s.Atualizar(id, req))
            .ThrowsAsync(new EntityNotFoundException("Funcionário não encontrado"));

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _controller.UpdateFuncionario(id, req));

        Assert.Equal("Funcionário não encontrado", ex.Message);
    }

    [Fact]
    public async Task GetAllFuncionarios_Should_Return_Ok_With_Default_Pagination()
    {
        var mockResponse = new PaginationResponse<ResponseFuncionarioDTO>
        (
            new List<ResponseFuncionarioDTO>
            {
                new ResponseFuncionarioDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Phone = "44999999999",
                    CPF = "68714247097",
                    Email = "teste@email.com",
                    Role = Role.GERENTE,
                    IsActive = true
                },
                new ResponseFuncionarioDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "rogerio",
                    Phone = "44888888888",
                    CPF = "78945612300",
                    Email = "rogerio@email.com",
                    Role = Role.GERENTE,
                    IsActive = true
                }
            },
            1,
            10,
            2
        );

        _mockService.Setup(s => s.ObterTodosFuncionarios(1, 10))
            .ReturnsAsync(ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>.Ok(mockResponse));

        var result = await _controller.GetAllFuncionarios();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>>(okResult.Value);

        Assert.True(apiResponse.Success);
        Assert.Equal(2, apiResponse.Data!.Items.Count);
        Assert.Equal("Admin", apiResponse.Data.Items[0].Name);
        Assert.Equal("rogerio@email.com", apiResponse.Data.Items[1].Email);
    }

    [Fact]
    public async Task GetAllFuncionarios_Should_Call_Service_ObterTodosFuncionarios()
    {
        _mockService.Setup(s => s.ObterTodosFuncionarios(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(It.IsAny<ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>>());

        var result = await _controller.GetAllFuncionarios();

        _mockService.Verify(s => s.ObterTodosFuncionarios(
            It.IsAny<int>(), 
            It.IsAny<int>()),
            Times.Once());
    }
}