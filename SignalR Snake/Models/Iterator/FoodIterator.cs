using System;
using System.Collections.Generic;
using SignalR_Snake.Models;

namespace SignalR_Snake.Models.Iterator
{
    public class FoodIterator : IIterator<Food>
    {
        private readonly List<Food> _foods;
        private int _index;

        public FoodIterator(List<Food> foods)
        {
            _foods = foods;
            _index = 0;
        }

        public bool HasNext() => _index < _foods.Count;

        public Food Next() => _foods[_index++];
    }
}
