using System;
using System.Collections.Generic;
using SignalR_Snake.Models;

namespace SignalR_Snake.Models.Iterator
{
    public class ObstacleAggregate : IAggregate<Obstacle>
    {
        private readonly List<Obstacle> _obstacles;

        public ObstacleAggregate(List<Obstacle> obstacles)
        {
            _obstacles = obstacles;
        }

        public IIterator<Obstacle> CreateIterator() => new ObstacleIterator(_obstacles);
    }
}
