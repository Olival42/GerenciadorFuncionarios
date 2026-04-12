using Xunit;
using Moq;
using GerenciadorFuncionarios.Modules.Funcionario.Domain.Repositories;
using GerenciadorFuncionarios.Modules.Funcionario.Web.Dtos.Responses;
using GerenciadorFuncionarios.Modules.Auth.Domain.Enums;
using GerenciadorFuncionarios.Shared.Responses;
using GerenciadorFuncionarios.Modules.Funcionario.Application.UseCases;

public class ObterTodosFuncionariosUseCaseTests
{
    private readonly Mock<IFuncionarioRepository> _mockRepository;

    private readonly ObterTodosFuncionarios _useCase;

    public ObterTodosFuncionariosUseCaseTests()
    {
        _mockRepository = new Mock<IFuncionarioRepository>();

        _useCase = new ObterTodosFuncionarios(_mockRepository.Object);
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
                    UserName = "admin",
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
    }

    [Fact]
    public async Task Execute_Should_ReturnPaginatedResult_WhenValid()
    {
        var paginated = Create_Paginated();

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(paginated);

        var result = await _useCase.Execute(1, 10);

        Assert.NotNull(result);
        Assert.Equal(
            paginated.Items.Select(x => x.Id),
            result.Data!.Items.Select(x => x.Id));
    }

    [Fact]
    public async Task Execute_Should_CallRepository_Once()
    {
        var paginated = Create_Paginated();

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(paginated);

        var result = await _useCase.Execute(1, 10);

        _mockRepository.Verify(
            r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.Once);
    }

    [Fact]
    public async Task Execute_Should_CallRepository_WithCorrectParams()
    {
        var paginated = Create_Paginated();

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(paginated);

        var result = await _useCase.Execute(1, 10);

        _mockRepository.Verify(
            r => r.GetAllAsync(1, 10),
            Times.Once);
    }

    [Fact]
    public async Task Execute_Should_ReturnSuccessTrue()
    {
        var paginated = Create_Paginated();

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(paginated);

        var result = await _useCase.Execute(1, 10);

        Assert.True(result.Success);
    }
}
