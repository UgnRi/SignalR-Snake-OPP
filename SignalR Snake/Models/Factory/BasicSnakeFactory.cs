using System;
using System.Collections.Generic;
using System.Drawing;
using SignalR_Snake.Hubs;
using SignalR_Snake.Models.Builder;

namespace SignalR_Snake.Models.Factory
{
    public class BasicSnakeFactory : SnakeFactory
    {
        public override SnakeBuilder CreateSnakeBuilder(string name)
        {
            Random Rng = SnakeHub.Rng;
            Point start = new Point(Rng.Next(300, 700), Rng.Next(300, 700));
            string color = SnakeHub.RandomColor();

            var parts = GetCircleSnakeParts(start, color, name);

            return (SnakeBuilder)new SnakeBuilder()
                .SetName(name)
                .SetStartPosition(start)
                .SetColor(color)
                .AddParts(parts);
        }

        private List<SnekPart> GetCircleSnakeParts(Point start, string color, string name)
        {
            List<SnekPart> parts = new List<SnekPart>();

            parts.Add(new SnekPart
            {
                Color = color,
                Position = new Point(start.X, start.Y),
                Name = name,
                Shape = "circle"
            });

            for (int i = 1; i < 10; i++)
            {
                parts.Add(new SnekPart
                {
                    Color = color,
                    Position = new Point(start.X - (i * 6), start.Y - (i * 6)),
                    Shape = "circle"
                });
            }

            return parts;
        }
    }
}