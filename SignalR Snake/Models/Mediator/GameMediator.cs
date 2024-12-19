namespace SignalR_Snake.Models.Mediator
{
    using System.Collections.Generic;
    using System.Drawing;

    public class GameMediator : IGameMediator
    {
        private List<Snake> snakes = new List<Snake>();
        private List<Food> foods = new List<Food>();
        private List<Obstacle> obstacles = new List<Obstacle>();

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
            int w = snake.Width;
            return snake.Parts[0].Position.X - w <= food.Position.X &&
                   snake.Parts[0].Position.X + w >= food.Position.X &&
                   snake.Parts[0].Position.Y - w < food.Position.Y &&
                   snake.Parts[0].Position.Y + 2 * w > food.Position.Y;
        }

        public bool HandleObstacleCollision(Snake snake, Obstacle obstacle)
        {
            int hitboxSize = 20;
            return snake.Parts[0].Position.X >= obstacle.Position.X - hitboxSize &&
                   snake.Parts[0].Position.X <= obstacle.Position.X + hitboxSize &&
                   snake.Parts[0].Position.Y >= obstacle.Position.Y - hitboxSize &&
                   snake.Parts[0].Position.Y <= obstacle.Position.Y + hitboxSize;
        }

        private bool IsCollision(Point p1, Point p2, int threshold)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y) < threshold * threshold;
        }
    }
}