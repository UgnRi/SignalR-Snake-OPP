using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SignalR_Snake.Models.Strategies
{
    public interface IMovementStrategy
    {
        Point Move(Point currentPosition, double direction, int speed);
    }
}
