using SignalR_Snake.Models.TemplateMethod;

namespace SignalR_Snake.Models.Mediator
{
    using System.Collections.Generic;
    using System.Drawing;

    public class GameMediator : IGameMediator
    {
        private List<Snake> snakes = new List<Snake>();
        private List<Food> foods = new List<Food>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private readonly FoodCollisionHandler foodCollisionHandler = new FoodCollisionHandler();
        private readonly ObstacleCollisionHandler obstacleCollisionHandler = new ObstacleCollisionHandler();
        public void RegisterSnake(Snake snake)
        {
            snakes.Add(snake);
        }

        public void RegisterFood(Food food)
        {
            foods.Add(food);
        }

        public void RegisterObstacle(Obstacle obstacle)
        {
            obstacles.Add(obstacle);
        }

        public bool HandleSnakeCollision(Snake snake)
        {
            foreach (var otherSnake in snakes)
            {
                if (otherSnake == snake) continue;

                foreach (var part in otherSnake.Parts)
                {
                    if (IsCollision(snake.Parts[0].Position, part.Position, snake.Width))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HandleFoodCollection(Snake snake, Food food)
        {
            return foodCollisionHandler.HandleCollision(snake, food);
        }

        public bool HandleObstacleCollision(Snake snake, Obstacle obstacle)
        {
            return obstacleCollisionHandler.HandleCollision(snake, obstacle);
        }

        private bool IsCollision(Point p1, Point p2, int threshold)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y) < threshold * threshold;
        }
    }
}