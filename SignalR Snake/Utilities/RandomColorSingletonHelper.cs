using System;
using System.Diagnostics;

namespace SignalR_Snake.Utilities
{
    public class RandomColorSingletonHelper
    {
        private static readonly Lazy<RandomColorSingletonHelper> _instance
            = new Lazy<RandomColorSingletonHelper>(() =>
            {
                Debug.WriteLine("RandomColorSingletonHelper instance is being created.");
                return new RandomColorSingletonHelper();
            },
            System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly Random _random;

        private RandomColorSingletonHelper()
        {
            _random = new Random();
            Debug.WriteLine("RandomColorSingleton initialized.");
        }

        public static RandomColorSingletonHelper Instance
        {
            get
            {
                Debug.WriteLine("RandomColorSingletonHelper.Instance accessed.");
                return _instance.Value;
            }
        }

        public string GenerateRandomColor()
        {
            Debug.WriteLine("GenerateRandomColor() called.");
            return $"#{_random.Next(0x1000000):X6}";
        }
    }
}