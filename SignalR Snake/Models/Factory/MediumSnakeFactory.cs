using System;
using System.Collections.Generic;
using System.Drawing;
using SignalR_Snake.Hubs;
using SignalR_Snake.Models.Builder;

namespace SignalR_Snake.Models.Factory
{
    public class MediumSnakeFactory : SnakeFactory
    {
        public override SnakeBuilder CreateSnakeBuilder(string name)
        {
            Random Rng = SnakeHub.Rng;
            Point start = new Point(Rng.Next(300, 700), Rng.Next(300, 700));
            string color = SnakeHub.RandomColor();

            var parts = GetMediumSnakeParts(start, color, name);

            return (SnakeBuilder)new SnakeBuilder()
                .SetName(name)
                .SetStartPosition(start)
                .SetColor(color)
                .AddParts(parts);
        }

        private List<SnekPart> GetMediumSnakeParts(Point start, string color, string name)
        {
            List<SnekPart> parts = new List<SnekPart>();

            parts.Add(new SnekPart
            {
                Color = color,
                Position = new Point(start.X, start.Y),
                Name = name
            });

            for (int i = 1; i < 15; i++)
            {
                parts.Add(new SnekPart
                {
                    Color = color,
                    Position = new Point(start.X - (i * 6), start.Y - (i * 6)),
                });
            }

            return parts;
        }
    }
}