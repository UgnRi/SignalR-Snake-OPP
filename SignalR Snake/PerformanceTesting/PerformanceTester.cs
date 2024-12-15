using System;
using System.Diagnostics;

namespace SignalR_Snake.PerformanceTesting.Flyweight
{
    public static class PerformanceTester
    {
        public static void MeasurePerformance(Action testMethod, string testName)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long beforeMemory = GC.GetTotalMemory(true);

            Stopwatch stopwatch = Stopwatch.StartNew();
            testMethod.Invoke();
            stopwatch.Stop();

            long afterMemory = GC.GetTotalMemory(true);

            Console.WriteLine($"Test: {testName}");
            Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Memory Usage: {afterMemory - beforeMemory} bytes");
            Console.WriteLine("----------------------------------------");
        }
    }
}
