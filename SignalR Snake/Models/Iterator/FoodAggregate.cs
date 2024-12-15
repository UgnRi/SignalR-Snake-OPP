using System;
using System.Collections.Generic;
using SignalR_Snake.Models;

namespace SignalR_Snake.Models.Iterator
{
    public class FoodAggregate : IAggregate<Food>
    {
        private readonly List<Food> _foods;

        public FoodAggregate(List<Food> foods)
        {
            _foods = foods;
        }

        public IIterator<Food> CreateIterator() => new FoodIterator(_foods);
    }
}
