namespace SignalR_Snake.Models.TemplateMethod
{
    public abstract class BaseCollisionHandler
    {
        public bool HandleCollision(Snake snake, object target)
        {
            PreProcessCollision(snake, target);
            
            bool collisionDetected = CheckCollision(snake, target);
            
            if (collisionDetected)
            {
                PostProcessCollision(snake);
            }
            
            return collisionDetected;
        }

        protected virtual void PreProcessCollision(Snake snake, object target) { }
        protected virtual void PostProcessCollision(Snake snake) { }

        protected abstract bool CheckCollision(Snake snake, object target);
    }
}