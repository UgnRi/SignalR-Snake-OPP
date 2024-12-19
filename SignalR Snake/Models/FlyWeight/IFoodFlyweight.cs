using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Snake.Models.FlyWeight
{
    internal interface IFoodFlyweight
    {
        public interface IFood : ICloneable
        {
            string Color { get; set; }
            Point Position { get; set; }
            void Initialize(string color, Point position);
        }
    }
}
