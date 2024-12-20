using System.Threading.Tasks;

namespace SignalR_Snake.Models.TemplateMethod
{
    public sealed class FoodCollisionHandler : BaseCollisionHandler
    {
        protected override bool CheckCollision(Snake snake, object target)
        {
            Food food = target as Food;
            if (food == null) 
                return false;
            
            int w = snake.Width;
            return snake.Parts[0].Position.X - w <= food.Position.X &&
                   snake.Parts[0].Position.X + w >= food.Position.X &&
                   snake.Parts[0].Position.Y - w < food.Position.Y &&
                   snake.Parts[0].Position.Y + 2 * w > food.Position.Y;
        }
        

        protected override async void PostProcessCollision(Snake snake)
        {
            
            snake.Speed = 6;

            // Wait for 1 second (1000 milliseconds)
            await Task.Delay(1000);

            // Restore original speed
            snake.Speed = 4;
        }
    }
}