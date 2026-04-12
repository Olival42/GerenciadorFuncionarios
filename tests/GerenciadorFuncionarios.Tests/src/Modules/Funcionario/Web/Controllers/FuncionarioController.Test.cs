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
using GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

public class FuncionarioControllerTests
{
    private readonly Mock<IRegistrarFuncionario> _mockRegistrarFuncionario;
    private readonly Mock<IObterFuncionarioLogado> _mockObterFuncionarioLogado;
    private readonly Mock<IObterFuncionarioPorId> _mockObterFuncionarioPorId;
    private readonly Mock<IInativarFuncionario> _mockInativarFuncionario;
    private readonly Mock<IAtualizarFuncionario> _mockAtualizarFuncionario;
    private readonly Mock<IObterTodosFuncionarios> _mockObterTodosFuncionarios;
    private readonly FuncionarioController _controller;

    public FuncionarioControllerTests()
    {
        _mockRegistrarFuncionario = new Mock<IRegistrarFuncionario>();
        _mockObterFuncionarioLogado = new Mock<IObterFuncionarioLogado>();
        _mockObterFuncionarioPorId = new Mock<IObterFuncionarioPorId>();
        _mockInativarFuncionario = new Mock<IInativarFuncionario>();
        _mockAtualizarFuncionario = new Mock<IAtualizarFuncionario>();
        _mockObterTodosFuncionarios = new Mock<IObterTodosFuncionarios>();
        _controller = new FuncionarioController(
            _mockRegistrarFuncionario.Object,
            _mockObterFuncionarioLogado.Object,
            _mockObterFuncionarioPorId.Object,
            _mockInativarFuncionario.Object,
            _mockAtualizarFuncionario.Object,
            _mockObterTodosFuncionarios.Object);

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
            UserName = "teste",
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
            UserName = "teste",
        };
    }

    [Fact]
    public async Task GetMeAsync_Should_Return_Ok_When_Authenticated()
    {
        var res = Create_ResponseDto();
        _mockObterFuncionarioLogado.Setup(s => s.Execute())
            .ReturnsAsync(ApiResponse<ResponseFuncionarioDTO>.Ok(res));

        var result = await _controller.GetMeAsync();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var apiResponse = Assert.IsType<ApiResponse<ResponseFuncionarioDTO>>(okResult.Value);

        Assert.Equal(res.Id, apiResponse.Data!.Id);
        Assert.Equal(res.Name, apiResponse.Data.Name);
        Assert.Equal(res.UserName, apiResponse.Data.UserName);
    }

    [Fact]
    public async Task GetMeAsync_Should_Call_Service_ObterFuncionarioLogadoAsync()
    {
        var res = Create_ResponseDto();
        _mockObterFuncionarioLogado.Setup(s => s.Execute())
            .ReturnsAsync(ApiResponse<ResponseFuncionarioDTO>.Ok(res));

        await _controller.GetMeAsync();

        _mockObterFuncionarioLogado.Verify(s => s.Execute(), Times.Once);
    }

    [Fact]
    public async Task GetMeAsync_Should_Throw_UnauthorizedException_When_Not_Authenticated()
    {
        _mockObterFuncionarioLogado.Setup(s => s.Execute())
            .ThrowsAsync(new UnauthorizedAccessException("Usuário não autenticado"));

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _controller.GetMeAsync());

        Assert.Equal("Usuário não autenticado", ex.Message);
    }

    [Fact]
    public async Task GetMeAsync_Should_Throw_EntityNotFoundException_When_Funcionario_Not_Found()
    {
        _mockObterFuncionarioLogado.Setup(s => s.Execute())
            .ThrowsAsync(new EntityNotFoundException("Funcionário não encontrado"));

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _controller.GetMeAsync());

        Assert.Equal("Funcionário não encontrado", ex.Message);
    }

    [Fact]
    public async Task GetMeAsync_Should_Throw_BadRequestException_When_UserId_Invalid()
    {
        _mockObterFuncionarioLogado.Setup(s => s.Execute())
            .ThrowsAsync(new BadRequestException("ID de usuário inválido"));

        var ex = await Assert.ThrowsAsync<BadRequestException>(
            () => _controller.GetMeAsync());

        Assert.Equal("ID de usuário inválido", ex.Message);
    }

    [Fact]
    public async Task Register_Should_Return_Created_With_Valid_Data()
    {
        var req = Create_RegisterDto();

        var res = Create_ResponseDto();

        _mockRegistrarFuncionario.Setup(s => s.Execute(req))
            .ReturnsAsync(ApiResponse<ResponseFuncionarioDTO>.Ok(res));

        var result = await _controller.Resgister(req);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var apiResponse = Assert.IsType<ApiResponse<ResponseFuncionarioDTO>>(createdResult.Value);

        Assert.Equal(res.Name, apiResponse.Data!.Name);
        Assert.Equal(res.UserName, apiResponse.Data.UserName);
        Assert.Equal(Role.GERENTE, apiResponse.Data.Role);
    }

    [Fact]
    public async Task Register_Should_Call_Service_RegistrarFuncionarioAsync()
    {
        var req = Create_RegisterDto();

        var res = Create_ResponseDto();

        _mockRegistrarFuncionario.Setup(s => s.Execute(req))
            .ReturnsAsync(ApiResponse<ResponseFuncionarioDTO>.Ok(res));

        var result = await _controller.Resgister(req);

        _mockRegistrarFuncionario.Verify(s => s.Execute(
            req),
            Times.Once());
    }

    [Fact]
    public async Task GetFuncionarioById_Should_Return_Ok_With_Valid_Id()
    {
        var funcionarioId = Guid.NewGuid();
        var responseDto = Create_ResponseDto();

        _mockObterFuncionarioPorId.Setup(s => s.Execute(funcionarioId))
            .ReturnsAsync(ApiResponse<ResponseFuncionarioDTO>.Ok(responseDto));

        var result = await _controller.GetFuncionarioById(funcionarioId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var apiResponse = Assert.IsType<ApiResponse<ResponseFuncionarioDTO>>(okResult.Value);

        Assert.Equal(funcionarioId, apiResponse.Data!.Id);
        Assert.Equal(responseDto.Name, apiResponse.Data.Name);
    }

    [Fact]
    public async Task GetFuncionarioById_Should_Return_NotFound_When_Id_Not_Exist()
    {
        _mockObterFuncionarioPorId.Setup(s => s.Execute(It.IsAny<Guid>()))
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

        _mockObterFuncionarioPorId.Setup(s => s.Execute(funcionarioId))
            .ReturnsAsync(ApiResponse<ResponseFuncionarioDTO>.Ok(responseDto));

        var result = await _controller.GetFuncionarioById(funcionarioId);

        _mockObterFuncionarioPorId.Verify(s => s.Execute(
            funcionarioId),
            Times.Once());
    }

    [Fact]
    public async Task InactiveById_Should_Return_NoContent_When_Successful()
    {
        var funcionarioId = Guid.NewGuid();

        _mockInativarFuncionario.Setup(s => s.Execute(funcionarioId));

        var result = await _controller.InactiveById(funcionarioId);

        var noContentResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task InactiveById_Should_Call_Service_InativarPorId()
    {
        _mockInativarFuncionario.Setup(s => s.Execute(It.IsAny<Guid>()));

        var result = await _controller.InactiveById(Guid.NewGuid());

        _mockInativarFuncionario.Verify(s => s.Execute(
            It.IsAny<Guid>()),
            Times.Once());
    }

    [Fact]
    public async Task InactiveById_Should_Return_NotFound_When_Id_Not_Exist()
    {
        _mockInativarFuncionario.Setup(s => s.Execute(It.IsAny<Guid>()))
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

        _mockAtualizarFuncionario.Setup(s => s.Execute(id, req))
            .ReturnsAsync(ApiResponse<ResponseFuncionarioDTO>.Ok(res));

        var result = await _controller.UpdateFuncionario(id, req);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var apiResponse = Assert.IsType<ApiResponse<ResponseFuncionarioDTO>>(okResult.Value);

        Assert.Equal(res.Name, apiResponse.Data!.Name);
        Assert.Equal(res.UserName, apiResponse.Data.UserName);
    }

    [Fact]
    public async Task UpdateFuncionario_Should_Call_Service_Atualizar()
    {
        var id = Guid.NewGuid();
        var req = new UpdateFuncionarioDTO
        {
            Name = "Admin",
        };

        _mockAtualizarFuncionario.Setup(s => s.Execute(id, req))
            .ReturnsAsync(It.IsAny<ApiResponse<ResponseFuncionarioDTO>>());

        var result = await _controller.UpdateFuncionario(id, req);

        _mockAtualizarFuncionario.Verify(s => s.Execute(
            id,
            req),
            Times.Once());
    }

    [Fact]
    public async Task UpdateFuncionario_Should_Return_NotFound_When_Id_Not_Exist()
    {
        var id = Guid.NewGuid();
        var req = new UpdateFuncionarioDTO { Name = "Admin" };

        _mockAtualizarFuncionario.Setup(s => s.Execute(id, req))
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
                    UserName = "teste",
                    Role = Role.GERENTE,
                    IsActive = true
                },
                new ResponseFuncionarioDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "rogerio",
                    Phone = "44888888888",
                    CPF = "78945612300",
                    UserName = "rogerio",
                    Role = Role.GERENTE,
                    IsActive = true
                }
            },
            1,
            10,
            2
        );

        _mockObterTodosFuncionarios.Setup(s => s.Execute(1, 10))
            .ReturnsAsync(ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>.Ok(mockResponse));

        var result = await _controller.GetAllFuncionarios();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>>(okResult.Value);

        Assert.True(apiResponse.Success);
        Assert.Equal(2, apiResponse.Data!.Items.Count);
        Assert.Equal("Admin", apiResponse.Data.Items[0].Name);
        Assert.Equal("rogerio", apiResponse.Data.Items[1].UserName);
    }

    [Fact]
    public async Task GetAllFuncionarios_Should_Call_Service_ObterTodosFuncionarios()
    {
        _mockObterTodosFuncionarios.Setup(s => s.Execute(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(It.IsAny<ApiResponse<PaginationResponse<ResponseFuncionarioDTO>>>());

        var result = await _controller.GetAllFuncionarios();

        _mockObterTodosFuncionarios.Verify(s => s.Execute(
            It.IsAny<int>(), 
            It.IsAny<int>()),
            Times.Once());
    }
}