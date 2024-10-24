using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SignalR_Snake.Models.Strategies
{
    public class NormalMovementStrategy : IMovementStrategy
    {
        public Point Move(Point currentPosition, double direction, int speed)
        {
            return new Point(
                currentPosition.X + (int)(Math.Cos(direction * (Math.PI / 180)) * speed),
                currentPosition.Y + (int)(Math.Sin(direction * (Math.PI / 180)) * speed)
            );
        }
    }
}
