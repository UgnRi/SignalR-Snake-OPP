using System;
using System.Threading.Tasks;

namespace SignalR_Snake.Models.TemplateMethod
{
    public sealed class ObstacleCollisionHandler : BaseCollisionHandler
    {
        private const int HITBOX_SIZE = 20;
        private const float SLOWDOWN_DISTANCE_THRESHOLD = 50.0f;
        protected override bool CheckCollision(Snake snake, object target)
        {
            Obstacle obstacle = target as Obstacle;
            if (obstacle == null)
                return false;
            
            return snake.Parts[0].Position.X >= obstacle.Position.X - HITBOX_SIZE &&
                   snake.Parts[0].Position.X <= obstacle.Position.X + HITBOX_SIZE &&
                   snake.Parts[0].Position.Y >= obstacle.Position.Y - HITBOX_SIZE &&
                   snake.Parts[0].Position.Y <= obstacle.Position.Y + HITBOX_SIZE;
        }

        protected override async void PreProcessCollision(Snake snake, object target)
        {
            var obstacle = target as Obstacle;
            if (obstacle != null)
            {
                bool isNearObstacle = 
                    snake.Parts[0].Position.X >= obstacle.Position.X - SLOWDOWN_DISTANCE_THRESHOLD &&
                    snake.Parts[0].Position.X <= obstacle.Position.X + SLOWDOWN_DISTANCE_THRESHOLD &&
                    snake.Parts[0].Position.Y >= obstacle.Position.Y - SLOWDOWN_DISTANCE_THRESHOLD &&
                    snake.Parts[0].Position.Y <= obstacle.Position.Y + SLOWDOWN_DISTANCE_THRESHOLD;
                
                if (isNearObstacle)
                {
                    snake.Speed = 2;
                    
                    await Task.Delay(2000);
                    
                    snake.Speed = 4;
                }
            }
        }
    }
}