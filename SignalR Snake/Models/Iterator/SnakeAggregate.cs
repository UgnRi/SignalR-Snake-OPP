using System;
using System.Collections.Generic;
using SignalR_Snake.Models;

namespace SignalR_Snake.Models.Iterator
{
    public class SnakeAggregate : IAggregate<Snake>
    {
        private readonly List<Snake> _snakes;

        public SnakeAggregate(List<Snake> snakes)
        {
            _snakes = snakes;
        }

        public IIterator<Snake> CreateIterator() => new SnakeIterator(_snakes);
    }
}
