using System.Collections.Generic;
using SignalR_Snake.Models;
using SignalR_Snake.Models.FlyWeight;

namespace SignalR_Snake.Models.Flyweight
{
    public class FoodFlyweightFactory
    {
        private readonly Dictionary<string, IFoodFlyweight> _foodFlyweights =
            new Dictionary<string, IFoodFlyweight>();

        public IFoodFlyweight GetFood(string color)
        {
            if (!_foodFlyweights.ContainsKey(color))
            {
                var newFood = new Food { Color = color };
                _foodFlyweights[color] = newFood;
            }
            return _foodFlyweights[color];
        }
    }
}