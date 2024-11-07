using System.Collections.Generic;
using System.Drawing;
using SignalR_Snake.Hubs;

namespace SignalR_Snake.Models.Builder
{
    public class SnakeBuilder : ISnakeBuilder
    {
        private string _name;
        private Point _startPosition;
        private string _color;
        private List<SnekPart> _parts = new List<SnekPart>();

        public ISnakeBuilder SetName(string name)
        {
            _name = name;
            return this;
        }

        public ISnakeBuilder SetStartPosition(Point startPosition)
        {
            _startPosition = startPosition;
            return this;
        }

        public ISnakeBuilder SetColor(string color)
        {
            _color = color;
            return this;
        }

        public ISnakeBuilder AddParts(List<SnekPart> parts)
        {
            _parts.AddRange(parts);
            return this;
        }

        public Snake Build(string connectionId)
        {
            return new Snake
            {
                Name = _name,
                ConnectionId = connectionId,
                Direction = 0,
                Parts = _parts,
                Width = 5,
                Color = _color
            };
        }
    }
}