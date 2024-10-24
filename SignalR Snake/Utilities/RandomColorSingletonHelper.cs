using System;

namespace SignalR_Snake.Utilities
{
    public class RandomColorSingletonHelper
    {
        private static readonly Lazy<RandomColorSingletonHelper> _instance 
            = new Lazy<RandomColorSingletonHelper>(() => new RandomColorSingletonHelper());

        private readonly Random _random;

        private RandomColorSingletonHelper()
        {
            _random = new Random();
            Console.WriteLine("RandomColorSingleton initialized.");
        }

        public static RandomColorSingletonHelper Instance => _instance.Value;

        public string GenerateRandomColor()
        {
            return $"#{_random.Next(0x1000000):X6}";
        }
    }
}