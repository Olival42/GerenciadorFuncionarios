namespace GerenciadorFuncionarios.Modules.Produto.Web.Hubs;

using GerenciadorFuncionarios.Modules.Produto.Web.Controllers.Dtos.Responses;
using Microsoft.AspNetCore.SignalR;

public class EstoqueHub : Hub
{
    public const string ALERTA_EVENT = "ReceberAlertaProduto";

    public async Task EnviarAlertaProduto(ProdutoAlertaDTO produto)
    {
        await Clients.All.SendAsync(ALERTA_EVENT, produto);
    }
}