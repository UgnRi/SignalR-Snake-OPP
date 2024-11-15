namespace SignalR_Snake.Models.AbstractFactory
{
    public abstract class SoundDecoratorAbstractFactory
    {
        public abstract void CreateBasicSnake(string name);
        public abstract void CreateTriangleSnake(string name);
        public abstract void CreateSquareSnake(string name);
        public abstract void CreateRandomSnake(string name);
    }
}