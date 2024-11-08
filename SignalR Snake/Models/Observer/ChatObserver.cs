using System;
using SignalR_Snake.Models;
using SignalR_Snake.Models.Observer;
using Microsoft.AspNet.SignalR;

namespace SignalR_Snake.Utilities
{
    public class ChatObserver : ISnakeObserver
    {
        private readonly IHubContext _hubContext;

        public ChatObserver()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalR_Snake.Hubs.SnakeHub>();
        }

        public void OnSnakeUpdated(Snake snake)
        {
            _hubContext.Clients.All.notifyChat($"{snake.Name} has been updated.");
        }

        public void OnSnakeDied(Snake snake)
        {
            _hubContext.Clients.All.notifyChat($"Snake {snake.Name} has died.");
        }
    }
}