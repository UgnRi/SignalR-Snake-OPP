using System.Collections.Generic;
using SignalR_Snake.Models;

namespace SignalR_Snake.Models.Flyweight
{
    public class FoodFlyweightFactory
    {
        private readonly Dictionary<string, Food> _foodFlyweights = new Dictionary<string, Food>();

        public Food GetFood(string color)
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
