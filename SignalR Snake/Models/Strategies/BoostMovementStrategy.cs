using System;
using System.Drawing;

namespace SignalR_Snake.Models.Strategies
{
    public class BoostMovementStrategy : IMovementStrategy
    {
        public Point Move(Point currentPosition, double direction, int speed)
        {
            return new Point(
                currentPosition.X + (int)(Math.Cos(direction * (Math.PI / 180)) * speed * 2),
                currentPosition.Y + (int)(Math.Sin(direction * (Math.PI / 180)) * speed * 2)
            );
        }
    }
}