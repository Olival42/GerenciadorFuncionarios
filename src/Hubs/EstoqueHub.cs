namespace GerenciadorFuncionarios.Hubs;

using Microsoft.AspNetCore.SignalR;
using GerenciadorFuncionarios.DTOs.Funcionario.Responses;
using GerenciadorFuncionarios.Adapters;

public class EstoqueHub : Hub, IEstoqueHub
{
    public async Task EnviarAlertaProduto(ProdutoAlertaDTO produto)
    {
        await Clients.All.SendAsync("ReceberAlertaPrsoduto", produto);
    }
}