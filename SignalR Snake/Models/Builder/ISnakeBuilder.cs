using System.Collections.Generic;
using System.Drawing;

namespace SignalR_Snake.Models.Builder
{
    public interface ISnakeBuilder
    {
        ISnakeBuilder SetName(string name);
        ISnakeBuilder SetStartPosition(Point startPosition);
        ISnakeBuilder SetColor(string color);
        ISnakeBuilder AddParts(List<SnekPart> parts);
        Snake Build(string connectionId);
    }
}