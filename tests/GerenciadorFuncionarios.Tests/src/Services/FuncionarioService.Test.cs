using Xunit;
using Moq;
using GerenciadorFuncionarios.Services;
using Microsoft.Extensions.Logging;
using GerenciadorFuncionarios.Adapters;
using GerenciadorFuncionarios.DTOs.Funcionario.Requests;
using GerenciadorFuncionarios.Enums;
using GerenciadorFuncionarios.Models;
using GerenciadorFuncionarios.Exceptions;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using Mapster;
using GerenciadorFuncionarios.Shared.Responses;

public class FuncionarioServiceTests
{
    private readonly Mock<IFuncionarioRepository> _mockFuncRepository;
    private readonly Mock<ILogger<FuncionarioService>> _mockLogger;

    private readonly FuncionarioService _funcService;

    public FuncionarioServiceTests()
    {
        _mockLogger = new Mock<ILogger<FuncionarioService>>();
        _mockFuncRepository = new Mock<IFuncionarioRepository>();

        _funcService = new FuncionarioService(
            _mockFuncRepository.Object,
            _mockLogger!.Object
        );
    }

    [Fact]
    public async Task RegistrarFuncionarioAsync_Should_Register_When_Data_Is_Valid()
    {
        var dto = Create_RegisterDto();

        _mockFuncRepository
            .Setup(r => r.AnyByCPFAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockFuncRepository
            .Setup(r => r.AnyByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _funcService.RegistrarFuncionarioAsync(dto);

        Assert.True(result.Success);

        _mockFuncRepository.Verify(
            r => r.Add(It.IsAny<Funcionario>()),
            Times.Once);
    }

    [Fact]
    public async Task RegistrarFuncionarioAsync_Should_Throw_When_CPF_Exists()
    {
        var dto = Create_RegisterDto();

        _mockFuncRepository
            .Setup(r => r.AnyByCPFAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<CPFAlreadyExistsException>(
            () => _funcService.RegistrarFuncionarioAsync(dto));

        Assert.Equal("CPF já cadastrado para outro funcionário.", exception.Message);
    }

    [Fact]
    public async Task RegistrarFuncionarioAsync_Should_Throw_When_Email_Exists()
    {
        var dto = Create_RegisterDto();

        _mockFuncRepository
            .Setup(r => r.AnyByCPFAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockFuncRepository
            .Setup(r => r.AnyByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<EmailAlreadyExistsException>(
            () => _funcService.RegistrarFuncionarioAsync(dto));

        Assert.Equal("Email já cadastrado para outro funcionário.", exception.Message);
    }

    [Fact]
    public async Task RegistrarFuncionarioAsync_Should_HashPassword()
    {
        var dto = Create_RegisterDto();
        Funcionario func = null!;

        _mockFuncRepository
            .Setup(r => r.AnyByCPFAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockFuncRepository
            .Setup(r => r.AnyByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockFuncRepository
            .Setup(r => r.Add(It.IsAny<Funcionario>()))
            .Callback<Funcionario>(f => func = f)
            .Returns(Task.CompletedTask);

        await _funcService.RegistrarFuncionarioAsync(dto);

        Assert.NotNull(func);
        Assert.NotEqual(dto.Password, func.PasswordHash);
        Assert.True(
            BCrypt.Net.BCrypt.Verify(dto.Password, func.PasswordHash)
        );
    }

    [Fact]
    public async Task RegistrarFuncionarioAsync_Should_Return_DTO()
    {
        var funcDto = Create_RegisterDto();

        _mockFuncRepository
            .Setup(r => r.AnyByCPFAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockFuncRepository
            .Setup(r => r.AnyByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _funcService.RegistrarFuncionarioAsync(funcDto);

        Assert.NotNull(result);
        Assert.True(result.Success);

        Assert.Equal(funcDto.Name, result.Data!.Name);
        Assert.Equal(funcDto.Email, result.Data!.Email);
        Assert.Equal(funcDto.Phone, result.Data!.Phone);
        Assert.Equal(funcDto.CPF, result.Data!.CPF);
        Assert.Equal(funcDto.Role, result.Data!.Role);
    }

    [Fact]
    public async Task ObterFuncionarioPorId_Should_ReturnFuncionario_WhenExists()
    {
        var func = Create_Funcionario();

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _funcService.ObterFuncionarioPorId(func.Id);

        Assert.NotNull(result);
        Assert.True(result.Success);

        Assert.Equal(func.Id, result.Data!.Id);
        Assert.Equal(func.Name, result.Data!.Name);
        Assert.Equal(func.Email, result.Data!.Email);
        Assert.Equal(func.Phone, result.Data!.Phone);
        Assert.Equal(func.CPF, result.Data!.CPF);
        Assert.Equal(func.Role, result.Data!.Role);
        Assert.Equal(func.IsActive, result.Data!.IsActive);
    }

    [Fact]
    public async Task ObterFuncionarioPorId_Should_Throw_WhenNotFound()
    {
        var id = Guid.NewGuid();

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Funcionario?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _funcService.ObterFuncionarioPorId(id)
        );

        Assert.Equal("Funcionário não encontrado.", ex.Message);
    }

    [Fact]
    public async Task ObterFuncionarioPorId_Should_CallRepository_WithCorrectId()
    {
        var func = Create_Funcionario();

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _funcService.ObterFuncionarioPorId(func.Id);

        _mockFuncRepository.Verify(
            r => r.GetByIdAsync(func.Id),
            Times.Once);
    }

    [Fact]
    public async Task ObterFuncionarioPorId_Should_ReturnSuccessTrue()
    {
        var func = Create_Funcionario();

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _funcService.ObterFuncionarioPorId(func.Id);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task ObterFuncionarioPorId_Should_MapFieldsCorrectly()
    {
        var func = Create_Funcionario();

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _funcService.ObterFuncionarioPorId(func.Id);

        Assert.Equal(func.Id, result.Data!.Id);
        Assert.Equal(func.Name, result.Data!.Name);
        Assert.Equal(func.Email, result.Data!.Email);
        Assert.Equal(func.Phone, result.Data!.Phone);
        Assert.Equal(func.CPF, result.Data!.CPF);
        Assert.Equal(func.Role, result.Data!.Role);
        Assert.Equal(func.IsActive, result.Data!.IsActive);
    }

    [Fact]
    public async Task InativarPorId_Should_SetIsActiveFalse_WhenFuncionarioExists()
    {
        var func = Create_Funcionario();

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        await _funcService.InativarPorId(func.Id);

        Assert.False(func.IsActive);
    }

    [Fact]
    public async Task InativarPorId_Should_CallSaveChanges_WhenFuncionarioExists()
    {
        var func = Create_Funcionario();

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        await _funcService.InativarPorId(func.Id);

        _mockFuncRepository.Verify(
            r => r.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task InativarPorId_Should_Throw_WhenFuncionarioNotFound()
    {
        var id = Guid.NewGuid();

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Funcionario?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _funcService.InativarPorId(id));

        Assert.Equal("Funcionário não encontrado.", ex.Message);
    }

    [Fact]
    public async Task InativarPorId_Should_CallRepository_WithCorrectId()
    {
        var func = Create_Funcionario();

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        await _funcService.InativarPorId(func.Id);

        _mockFuncRepository.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>()),
            Times.Once);
    }

    [Fact]
    public async Task InativarPorId_Should_NotCallSaveChanges_WhenFuncionarioNotFound()
    {
        var id = Guid.NewGuid();

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Funcionario?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _funcService.InativarPorId(id));

        _mockFuncRepository.Verify(
            r => r.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task ObterTodosFuncionarios_Should_ReturnPaginatedResult_WhenValid()
    {
        var paginated = Create_Paginated();

        _mockFuncRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(paginated);

        var result = await _funcService.ObterTodosFuncionarios(1, 10);

        Assert.NotNull(result);
        Assert.Equal(
            paginated.Items.Select(x => x.Id),
            result.Data!.Items.Select(x => x.Id));
    }

    [Fact]
    public async Task ObterTodosFuncionarios_Should_CallRepository_Once()
    {
        var paginated = Create_Paginated();

        _mockFuncRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(paginated);

        var result = await _funcService.ObterTodosFuncionarios(1, 10);

        _mockFuncRepository.Verify(
            r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterTodosFuncionarios_Should_CallRepository_WithCorrectParams()
    {
        var paginated = Create_Paginated();

        _mockFuncRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(paginated);

        var result = await _funcService.ObterTodosFuncionarios(1, 10);

        _mockFuncRepository.Verify(
            r => r.GetAllAsync(1, 10),
            Times.Once);
    }

    [Fact]
    public async Task ObterTodosFuncionarios_Should_ReturnSuccessTrue()
    {
        var paginated = Create_Paginated();

        _mockFuncRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(paginated);

        var result = await _funcService.ObterTodosFuncionarios(1, 10);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Atualizar_Should_UpdateAllFields_WhenAllProvided()
    {
        var func = Create_Funcionario();

        var request = new UpdateFuncionarioDTO
        {
            Name = "rogerio",
            Email = "rogerio@email.com",
            Password = "12345678",
            Role = Role.GERENTE,
            Phone = "44888888888",
            CPF = "78945612300"
        };

        _mockFuncRepository.
            Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _funcService.Atualizar(func.Id, request);

        Assert.Equal(request.Name, func.Name);
        Assert.Equal(request.Email, func.Email);
        Assert.True(BCrypt.Net.BCrypt.Verify(request.Password, func.PasswordHash));
        Assert.Equal(request.Role, func.Role);
        Assert.Equal(request.Phone, func.Phone);
        Assert.Equal(request.CPF, func.CPF);
    }

    [Fact]
    public async Task Atualizar_Should_UpdateOnlyProvidedFields()
    {
        var func = Create_Funcionario();

        var original = new
        {
            func.Name,
            func.Email,
            func.Role,
            func.Phone,
            func.CPF,
            func.PasswordHash
        };

        var request = new UpdateFuncionarioDTO
        {
            Name = "rogerio",
        };

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        await _funcService.Atualizar(func.Id, request);

        Assert.Equal("rogerio", func.Name);
        Assert.Equal(original.Email, func.Email);
        Assert.Equal(original.Role, func.Role);
        Assert.Equal(original.Phone, func.Phone);
        Assert.Equal(original.CPF, func.CPF);
        Assert.Equal(original.PasswordHash, func.PasswordHash);
    }

    [Fact]
    public async Task Atualizar_Should_Throw_WhenFuncionarioNotFound()
    {
        var request = new UpdateFuncionarioDTO
        {
            Name = "rogerio",
        };

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Funcionario?)null);

        var ex = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _funcService.Atualizar(Guid.NewGuid(), request));

        Assert.Equal("Funcionário não encontrado.", ex.Message);
    }

    [Fact]
    public async Task Atualizar_Should_CallSaveChanges_WhenValid()
    {
        var func = Create_Funcionario();

        var request = new UpdateFuncionarioDTO
        {
            Name = "rogerio",
        };

        _mockFuncRepository.
            Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _funcService.Atualizar(func.Id, request);

        _mockFuncRepository.Verify(
           f => f.SaveChangesAsync(),
           Times.Once
        );
    }

    [Fact]
    public async Task Atualizar_Should_CallRepository_WithCorrectId()
    {
        var func = Create_Funcionario();

        var request = new UpdateFuncionarioDTO
        {
            Name = "rogerio",
        };

        _mockFuncRepository.
            Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _funcService.Atualizar(func.Id, request);

        _mockFuncRepository.Verify(
           f => f.GetByIdAsync(func.Id),
           Times.Once
        );
    }

    [Fact]
    public async Task Atualizar_Should_NotChangeFields_WhenNull()
    {
        var func = Create_Funcionario();

        var original = new
        {
            func.Name,
            func.Email,
            func.Role,
            func.Phone,
            func.CPF,
            func.PasswordHash
        };

        var request = new UpdateFuncionarioDTO
        {
            Name = "Admin",
        };

        _mockFuncRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(func);

        var result = await _funcService.Atualizar(func.Id, request);

        Assert.Equal(original.Email, result.Data!.Email);
        Assert.Equal(original.Role, result.Data.Role);
        Assert.Equal(original.Phone, result.Data.Phone);
        Assert.Equal(original.CPF, result.Data.CPF);
        Assert.Equal(original.PasswordHash, func.PasswordHash);
    }

    private RegisterFuncionarioDTO Create_RegisterDto()
    {
        return new RegisterFuncionarioDTO
        {
            Name = "Admin",
            Email = "teste@email.com",
            Password = "123456",
            Role = Role.ADMIN,
            CPF = "12345678900",
            Phone = "44999999999",
        };
    }

    private Funcionario Create_Funcionario()
    {
        return new Funcionario
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Phone = "44999999999",
            CPF = "68714247097",
            Email = "teste@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = Role.ADMIN,
            IsActive = true
        };
    }

    private PaginationResponse<ResponseFuncionarioDTO> Create_Paginated()
    {
        return new PaginationResponse<ResponseFuncionarioDTO>
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
                    Role = Role.ADMIN,
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
    }
}