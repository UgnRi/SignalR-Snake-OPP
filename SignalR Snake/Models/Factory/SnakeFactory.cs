using System.Drawing;
using SignalR_Snake.Models.Builder;

namespace SignalR_Snake.Models.Factory
{
    public abstract class SnakeFactory
    {
        public abstract SnakeBuilder CreateSnakeBuilder(string name);
    }
}