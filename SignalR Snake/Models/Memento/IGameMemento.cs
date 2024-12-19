using System.Collections.Generic;

namespace SignalR_Snake.Models.Memento
{
    public interface IGameMemento
    {
        List<Snake> GetSnakes();
        List<Food> GetFoods();
        List<Obstacle> GetObstacles();
    }
}