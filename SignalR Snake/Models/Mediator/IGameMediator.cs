namespace SignalR_Snake.Models.Mediator
{
    public interface IGameMediator
    {
        void RegisterSnake(Snake snake);
        void RegisterFood(Food food);
        void RegisterObstacle(Obstacle obstacle);
        bool HandleSnakeCollision(Snake snake);
        bool HandleFoodCollection(Snake snake, Food food);
        bool HandleObstacleCollision(Snake snake, Obstacle obstacle);
    }
}