using System;
using System.Drawing;

namespace SignalR_Snake.Models.Strategies
{
    public class SlowMovementStrategy : IMovementStrategy
    {
        public Point Move(Point currentPosition, double direction, int speed)
        {
            return new Point(
                currentPosition.X + (int)(Math.Cos(direction * (Math.PI / 180)) * speed * 0.5),
                currentPosition.Y + (int)(Math.Sin(direction * (Math.PI / 180)) * speed * 0.5)
            );
        }
    }
}