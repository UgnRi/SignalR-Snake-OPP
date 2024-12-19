using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SignalR_Snake.Models.Memento
{
    public class GameStateMemento
    { 
        private readonly List<Snake> _snakes;
        private readonly List<Food> _foods;
        private readonly List<Obstacle> _obstacles;
        internal GameStateMemento(List<Snake> snakes, List<Food> foods, List<Obstacle> obstacles)
        {
            _snakes = snakes.Select(s => s.Clone()).ToList();
            _foods = foods.Select(f => (Food)f.Clone()).ToList();
            _obstacles = obstacles.Select(o => new Obstacle(o.Position, o.Color)).ToList();
        }
        internal List<Snake> GetSnakes() => _snakes.Select(s => s.Clone()).ToList();
        internal List<Food> GetFoods() => _foods.Select(f => (Food)f.Clone()).ToList();
        internal List<Obstacle> GetObstacles() => _obstacles.Select(o => new Obstacle(o.Position, o.Color)).ToList();
    }
}