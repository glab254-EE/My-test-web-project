using System;
using System.Net.WebSockets;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

public class WSClientsBehaviour : MonoBehaviour
{
    [SerializeField]
    private List<WSClientArgument> client1Arguments = new();
    [SerializeField]
    private List<WSClientArgument> client2Arguments = new();
    async void Awake()
    {
        ClientWebSocket Client1 = await GetClientSocket();
        ClientWebSocket Client2 = await GetClientSocket();

        Task Task1 = HandleClientArguments(client1Arguments, Client1);
        Task Task2 = HandleClientArguments(client2Arguments, Client2,"Client 2: ");

        await Task.WhenAll(Task1, Task2);

        await DiscconectSocket(Client1);
        await DiscconectSocket(Client2);
    }
    private async Task HandleClientArguments(List<WSClientArgument> arguments,ClientWebSocket socket, string sender = "Client 1: ")
    {
        Debug.Log(sender);
        for (int i = 0; i < arguments.Count; i++)
        {
            WSClientArgument argument = arguments[i];
            if (argument.ClientActionEnum == WSClientArgument.WSClientAction.AwaitMessage)
            {
                string message = await AwaitMessage(socket);
                Debug.Log(message);
            } else if (argument.ResponseString != null)
            {
                await SendText(socket,argument.ResponseString);
            }
        }
    }
    private async Task DiscconectSocket(ClientWebSocket socket)
    {
        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure,"",CancellationToken.None);
        Debug.Log("Left!");
    }
    private async Task<ClientWebSocket> GetClientSocket()
    {
        ClientWebSocket toReturn = new();
        var uri = new Uri("ws://localhost:7777");

        await toReturn.ConnectAsync(uri, CancellationToken.None);
        Debug.Log("Connected!");
        return toReturn;
    }
    private async Task SendText(ClientWebSocket client, string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        await client.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task<string> AwaitMessage(ClientWebSocket client)
    {
        var buffer = new byte[1024];

        var result = await client.ReceiveAsync(buffer, CancellationToken.None);

        return Encoding.UTF8.GetString(buffer, 0, result.Count);
    }
}
