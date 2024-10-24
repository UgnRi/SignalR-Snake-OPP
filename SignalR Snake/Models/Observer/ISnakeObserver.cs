using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Snake.Models.Observer
{
    public interface ISnakeObserver
    {
        void OnSnakeUpdated(Snake snake);
        void OnSnakeDied(Snake snake);
    }
}
