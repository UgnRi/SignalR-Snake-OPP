using System.Drawing;
using SignalR_Snake.Utilities;


namespace SignalR_Snake.Models
{
    public class Obstacle
    {
        public Point Position { get; set; }
        public string Color { get; set; }

        public Obstacle(Point position, string color)
        {
            Position = position;
            Color = color;
        }
    }
}
