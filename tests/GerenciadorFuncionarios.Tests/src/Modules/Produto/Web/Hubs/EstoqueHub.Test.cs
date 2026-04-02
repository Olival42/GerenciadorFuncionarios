using Xunit;
using Moq;
using Microsoft.AspNetCore.SignalR;
using GerenciadorFuncionarios.Hubs;
using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;

public class EstoqueHubTests
{
    private readonly Mock<IHubCallerClients> _clientsMock;
    private readonly Mock<IClientProxy> _clientProxyMock;
    private readonly EstoqueHub _hub;

    public EstoqueHubTests()
    {
        _clientsMock = new Mock<IHubCallerClients>();
        _clientProxyMock = new Mock<IClientProxy>();

        _clientsMock.Setup(c => c.All)
                    .Returns(_clientProxyMock.Object);

        _hub = new EstoqueHub
        {
            Clients = _clientsMock.Object
        };
    }

    [Fact]
    public async Task EnviarAlertaProduto_Should_Call_SendAsync()
    {
        var produto = new ProdutoAlertaDTO
        {
            Id = Guid.NewGuid(),
            Name = "Gas",
            Quantity = 5
        };

        await _hub.EnviarAlertaProduto(produto);

        _clientProxyMock.Verify(
            x => x.SendCoreAsync(
                EstoqueHub.ALERTA_EVENT,
                It.Is<object[]>(o => o.Length == 1 && (ProdutoAlertaDTO)o[0] == produto),
                default),
            Times.Once);
    }

    [Fact]
    public async Task EnviarAlertaProduto_Should_Use_Correct_Event_Name()
    {
        var produto = new ProdutoAlertaDTO
        {
            Id = Guid.NewGuid(),
            Name = "Agua",
            Quantity = 3
        };

        await _hub.EnviarAlertaProduto(produto);

        _clientProxyMock.Verify(
            x => x.SendCoreAsync(
                "ReceberAlertaProduto",
                It.IsAny<object[]>(),
                default),
            Times.Once);
    }

    [Fact]
    public async Task EnviarAlertaProduto_Should_Send_ProdutoDTO()
    {
        var produto = new ProdutoAlertaDTO
        {
            Id = Guid.NewGuid(),
            Name = "Agua",
            Quantity = 1
        };

        await _hub.EnviarAlertaProduto(produto);

        _clientProxyMock.Verify(
            x => x.SendCoreAsync(
                EstoqueHub.ALERTA_EVENT,
                It.Is<object[]>(o => (ProdutoAlertaDTO)o[0] == produto),
                default),
            Times.Once);
    }
}