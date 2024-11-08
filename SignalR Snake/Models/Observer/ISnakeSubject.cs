using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Snake.Models.Observer
{
    public interface ISnakeSubject
    {
        void RegisterObserver(ISnakeObserver observer);
        void RemoveObserver(ISnakeObserver observer);
        void NotifySnakeUpdated(Snake snake);
        void NotifySnakeDied(Snake snake);
    }
}