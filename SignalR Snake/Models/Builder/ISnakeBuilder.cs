using System.Drawing;

namespace SignalR_Snake.Models.Builder
{
    public interface ISnakeBuilder
    {
        ISnakeBuilder SetName(string name);
        ISnakeBuilder SetStartPosition(Point startPosition);
        ISnakeBuilder SetColor(string color);
        ISnakeBuilder AddPart(SnekPart part);
        Snake Build(string connectionId);
    }
}