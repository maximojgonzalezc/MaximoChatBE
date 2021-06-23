using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChatService;
using System.Linq;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string ChatBotURL = "http://localhost:7071/api/";
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;

        public ChatHub(IDictionary<string, UserConnection> connections)
        {
            _botUser = "MyChat Bot";
            _connections = connections;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has left");
                SendUsersConnected(userConnection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            _connections[Context.ConnectionId] = userConnection;

            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has joined {userConnection.Room}");

            await SendUsersConnected(userConnection.Room);
        }

        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", userConnection.User, message);

                if (IsvalidCommand(message))
                {
                    var stockCode = message.Replace("/stock=", "");
                    HttpClient newClient = new HttpClient();
                    HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, $"{ChatBotURL}ChatBot?stockName={stockCode}");
                    HttpResponseMessage response = await newClient.SendAsync(newRequest);
                    var parsedCSV = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(parsedCSV))
                    {
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{$"Stock code is not valid. StockCode = {stockCode}"}");
                    }else await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{parsedCSV}");
                }
            }
        }

        public Task SendUsersConnected(string room)
        {
            var users = _connections.Values
                .Where(c => c.Room == room)
                .Select(c => c.User);

            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }

        private bool IsvalidCommand(string message)
        {
            return message.StartsWith("/stock=");
        }
    }
}
