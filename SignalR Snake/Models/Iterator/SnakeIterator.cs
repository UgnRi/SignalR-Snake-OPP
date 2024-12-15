using System;
using System.Collections.Generic;
using SignalR_Snake.Models;

namespace SignalR_Snake.Models.Iterator
{
    public class SnakeIterator : IIterator<Snake>
    {
        private readonly List<Snake> _snakes;
        private int _index;

        public SnakeIterator(List<Snake> snakes)
        {
            _snakes = snakes;
            _index = 0;
        }

        public bool HasNext() => _index < _snakes.Count;

        public Snake Next() => _snakes[_index++];
    }
}
