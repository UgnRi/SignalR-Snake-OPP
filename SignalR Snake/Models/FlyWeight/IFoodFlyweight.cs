using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SignalR_Snake.Models.FlyWeight
{
    public interface IFoodFlyweight : ICloneable
    {
        string Color { get; set; }
        Point Position { get; set; }
        void Initialize(string color, Point position);
    }
}
