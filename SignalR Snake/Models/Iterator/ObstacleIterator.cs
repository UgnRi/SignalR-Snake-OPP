using System;
using System.Collections.Generic;
using SignalR_Snake.Models;

namespace SignalR_Snake.Models.Iterator
{
    public class ObstacleIterator : IIterator<Obstacle>
    {
        private readonly List<Obstacle> _obstacles;
        private int _index;

        public ObstacleIterator(List<Obstacle> obstacles)
        {
            _obstacles = obstacles;
            _index = 0;
        }

        public bool HasNext() => _index < _obstacles.Count;

        public Obstacle Next() => _obstacles[_index++];
    }
}
