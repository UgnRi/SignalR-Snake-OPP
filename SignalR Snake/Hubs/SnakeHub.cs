using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;
using System.Web.UI;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalR_Snake.Models;
using SignalR_Snake.Models.Flyweight;
using SignalR_Snake.Models.Sound;
using SignalR_Snake.Models.Memento;
using SignalR_Snake.Models.Builder;
using SignalR_Snake.Models.Iterator;
using SignalR_Snake.Models.Factory;
using SignalR_Snake.Models.Strategies;
using SignalR_Snake.Models.Observer;
using SignalR_Snake.Models.Command;
using SignalR_Snake.Utilities;
using Timer = System.Timers.Timer;
using System.Security.Cryptography;

namespace SignalR_Snake.Hubs
{
    public class SnakeHub : Hub, ISnakeObserver
    {
        public static List<Snake> Sneks = new List<Snake>();
        public static List<Food> Foods = new List<Food>();
        public static List<Obstacle> Obstacles = new List<Obstacle>();
        public static List<Projectile> Projectiles = new List<Projectile>();
        private static CommandInvoker commandInvoker = new CommandInvoker();
        private static IHubCallerConnectionContext<dynamic> clientsStatic;
        public static Random Rng = new Random();
        private static IGameSound gameSound;
        private static GameStateMemento savedState = null;
        private static bool isGameSaved = false;
        private static readonly FoodFlyweightFactory foodFactory = new FoodFlyweightFactory();

        public List<ISnakeObserver> observers = new List<ISnakeObserver>();

        public SnakeHub()
        {
            var chatObserver = new ChatObserver();
            RegisterObserver(chatObserver);
        }

        public void HoldProjectiles(double startX, double startY, double directionX, double directionY, double radius)
        {
            string ownerId = Context.ConnectionId;
            var command = new FireProjectileCommand(
                ownerId: ownerId,
                startX: startX,
                startY: startY,
                targetX: directionX,
                targetY: directionY,
                speed: 5,
                onProjectileCreated: (projectile) =>
                {
                    Projectiles.Add(projectile);

                    Clients.All.ReceiveProjectile(projectile);
                }
            );

            commandInvoker.AddCommand(command);
        }

        public void DrawAllProjectiles()
        {
            Clients.All.DrawAllReleasedProjectiles();
        }

        public void ReleaseAllProjectiles()
        {
            commandInvoker.ExecuteAll();
            DrawAllProjectiles();
        }

        public void PlayBackgroundSound()
        {
            gameSound = new BaseGameSound("sweden.wav");
            gameSound.PlaySound();
        }

        public void PlayFastForwardSound()
        {
            gameSound = new FFSoundDecorator(gameSound, "fast-forward.mp3");
            gameSound.PlaySound();
        }
        public void PlayLightningSound()
        {
            gameSound = new LightningSoundDecorator(gameSound, "lightning.wav");
            gameSound.PlaySound();
        }

        public void PlayCreeperSound()
        {
            gameSound = new CreeperSoundDecorator(gameSound, "creeper.wav");
            gameSound.PlaySound();
        }

        public void PlayTapSound()
        {
            gameSound = new TapSoundDecorator(gameSound, "tap.wav");
            gameSound.PlaySound();
        }

        public void PlayZombieSound()
        {
            gameSound = new ZombieSoundDecorator(gameSound, "zombie.wav");
            gameSound.PlaySound();
        }

        public void StopSound()
        {
            gameSound?.StopSound();
        }

        public void StopAllSounds()
        {
            gameSound = new TapSoundDecorator(gameSound, "tap.wav");
            TapSoundDecorator tap = (TapSoundDecorator)gameSound;
            tap.StopAllSounds();
        }

        public void RegisterObserver(ISnakeObserver observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(ISnakeObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifySnakeUpdated(Snake snake)
        {
            foreach (var observer in observers)
            {
                observer.OnSnakeUpdated(snake);
            }
            Clients.All.notifyChat($"{snake.Name} has eaten food");
        }
        public void NotifySnakeDied(Snake snake)
        {
            foreach (var observer in observers)
            {
                observer.OnSnakeDied(snake);
            }
        }
        public void OnSnakeUpdated(Snake snake)
        {
            Clients.All.notifyChat($"{snake.Name} was updated in SnakeHub.");
        }

        public void OnSnakeDied(Snake snake)
        {
            Clients.All.notifyChat($"Snake {snake.Name} has died in SnakeHub.");
        }
        public void NewSnek(string name, string shape)
        {
            SnakeFactory snakeFactory;
            switch (shape.ToLower())
            {
                case "random":
                    snakeFactory = new RandomSnakeFactory();
                    break;
                case "square":
                    snakeFactory = new SquareSnakeFactory();
                    break;
                case "triangle":
                    snakeFactory = new TriangleSnakeFactory();
                    break;
                case "basic":
                    snakeFactory = new BasicSnakeFactory();
                    break;
                default:
                    snakeFactory = new RandomSnakeFactory();
                    break;
            }

            SnakeBuilder snakeBuilder = snakeFactory.CreateSnakeBuilder(name);
            var newSnake = snakeBuilder.Build(Context.ConnectionId);

            lock (Sneks)
            {
                Sneks.Add(newSnake);
                NotifySnakeUpdated(newSnake);
            }
            clientsStatic = Clients;
        }


        static SnakeHub()
        {
            Timer timer = new Timer(5) {AutoReset = true, Enabled = true};
            timer.Elapsed += Timer_Elapsed;

            Timer moveTimer = new Timer(5) { AutoReset = true, Enabled = true };
            moveTimer.Elapsed += MoveTimer_Elapsed;            
        }

        private static Food PrototypeFood = new Food();
        private static void MoveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<SnakeHub>();
            lock (Sneks)
            {
                var snakesToRemove = new List<Snake>();
                foreach (var snek in Sneks)
                {
                    //Strategy


                    Point nextPosition = snek.MovementStrategy.Move(snek.Parts[0].Position, snek.Dir, snek.Speed);

                    for (int i = 0; i < snek.Parts.Count - 1; i++)
                    {
                        snek.Parts[snek.Parts.Count - (i + 1)].Position = snek.Parts[snek.Parts.Count - (2 + i)].Position;
                    }

                    snek.Parts[0].Position = nextPosition;
                    var obstacleAggregate = new ObstacleAggregate(Obstacles);
                    var obstacleIterator = obstacleAggregate.CreateIterator();

                    while (obstacleIterator.HasNext())
                    {
                        var obstacle = obstacleIterator.Next();
                        int hitboxSize = 20;

                        if (snek.Parts[0].Position.X >= obstacle.Position.X - hitboxSize &&
                            snek.Parts[0].Position.X <= obstacle.Position.X + hitboxSize &&
                            snek.Parts[0].Position.Y >= obstacle.Position.Y - hitboxSize &&
                            snek.Parts[0].Position.Y <= obstacle.Position.Y + hitboxSize)
                        {
                            hubContext.Clients.User(snek.ConnectionId).died();
                            snakesToRemove.Add(snek);
                            break;
                        }
                    }
                    lock (Foods)
                    {
                        for (int i = 0; i < 1000 - Foods.Count; i++)
                        {
                            string color = RandomColorSingletonHelper.Instance.GenerateRandomColor();
                            Food sharedFood = foodFactory.GetFood(color);
                            Console.WriteLine($"Shared Food Hash Code (Color: {color}): {sharedFood.GetHashCode()}");

                            Food newFood = (Food)sharedFood.Clone();
                            Console.WriteLine($"Cloned Food Hash Code: {newFood.GetHashCode()}");
                            newFood.Position = new Point(Rng.Next(0, 2000), Rng.Next(0, 2000));

                            Foods.Add(newFood);
                        }

                        var foodAggregate = new FoodAggregate(Foods);
                        var foodIterator = foodAggregate.CreateIterator();
                        List<Food> toRemove = new List<Food>();
                        while (foodIterator.HasNext())
                        {
                            var food = foodIterator.Next();
                            int w = snek.Width;

                            if (snek.Parts[0].Position.X - w <= food.Position.X &&
                                snek.Parts[0].Position.X + w >= food.Position.X &&
                                snek.Parts[0].Position.Y - w < food.Position.Y &&
                                snek.Parts[0].Position.Y + 2 * w > food.Position.Y)
                            {
                                snek.Parts.Add(snek.Parts[snek.Parts.Count - 1]);
                                snek.Parts.Add(snek.Parts[snek.Parts.Count - 1]);
                                snek.Parts.Add(snek.Parts[snek.Parts.Count - 1]);
                                toRemove.Add(food);
                            }
                        }
                        foreach (var food in toRemove)
                        {
                            Foods.Remove(food);
                        }
                    }
                    lock (Obstacles)
                    {
                        if (Obstacles.Count < 20)
                        {
                            Point position = new Point(Rng.Next(0, 2000), Rng.Next(0, 2000));
                            string color = RandomColorSingletonHelper.Instance.GenerateRandomColor();

                            Obstacles.Add(new Obstacle(position, color));
                        }
                    }
                }
                foreach (var snek in snakesToRemove)
                {
                    Sneks.Remove(snek);
                }
            }
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (Sneks)
            {
                var snakeAggregate = new SnakeAggregate(Sneks);
                var snakeIterator = snakeAggregate.CreateIterator();

                List<Snake> toRemoveSnakes = new List<Snake>();

                while (snakeIterator.HasNext())
                {
                    var snek = snakeIterator.Next();

                    // Check if the snake collides with another snake
                    bool collision = Sneks.Any(x => x.Parts.Any(c =>
                        c.Position.X > snek.Parts[0].Position.X - snek.Width &&
                        c.Position.X < snek.Parts[0].Position.X + snek.Width &&
                        c.Position.Y > snek.Parts[0].Position.Y - snek.Width &&
                        c.Position.Y < snek.Parts[0].Position.Y + snek.Width) && x != snek);

                    if (!collision)
                        continue;

                    // Notify the client that the snake has died
                    clientsStatic.User(snek.ConnectionId).Died();

                    // Add snake parts back as food
                    foreach (var part in snek.Parts)
                    {
                        Foods.Add(new Food() { Color = RandomColor(), Position = part.Position });
                    }

                    // Mark the snake for removal
                    toRemoveSnakes.Add(snek);
                    clientsStatic.User(snek.ConnectionId).Died();
                    break; // Stop after processing one collision
                }

                // Remove flagged snakes after iteration
                foreach (var snek in toRemoveSnakes)
                {
                    Sneks.Remove(snek);
                }
            }
        }


        public void AllPos()
        {
            List<SnekPart> snakeParts = new List<SnekPart>();
            Point myPoint = new Point(0, 0);
            lock (Sneks)
            {
                foreach (var snek in Sneks)
                {
                    if (snek.ConnectionId == Context.ConnectionId)
                    {
                        myPoint = snek.Parts[0].Position;
                    }
                    snakeParts.AddRange(snek.Parts);
                }
                snakeParts.Reverse();
                Clients.Caller.AllPos(snakeParts, myPoint, Foods, Obstacles);
            }
        }


        //public void MyPos()
        //{
        //    Point myPoint = Sneks.First(x => x.ConnectionId.Equals(Context.ConnectionId)).Position[0];
        //    Clients.Client(Context.ConnectionId).MyPos(myPoint);
        //}
        public void Speed()
        {
            lock (Sneks)
            {
                if (!Sneks.Any(x => x.ConnectionId.Equals(Context.ConnectionId))) return;
                    Snake snek = Sneks.First(x => x.ConnectionId.Equals(Context.ConnectionId));
                    snek.Fast = !snek.Fast;
            }
        }

        public void Score()
        {
            lock (Sneks)
            {
                List<SnekScore> snekScores =
                    Sneks.Select(snek => new SnekScore() {SnakeName = snek.Name, Length = snek.Parts.Count}).ToList();
                var ordered = snekScores.OrderByDescending(x => x.Length);
                Clients.Caller.Score(ordered);
            }
        }
        public void ChangeMovementStrategy(string strategyType)
        {
            lock (Sneks)
            {
                var snek = Sneks.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                if (snek != null)
                {
                    snek.ToggleMovementStrategy(strategyType);
                }
            }
        }
        //Singleton
        public static string RandomColor()
        {
            return RandomColorSingletonHelper.Instance.GenerateRandomColor();
        }
        //Thread safe demo
        public void GenerateRandomColorForClient()
        {
            string color = RandomColorSingletonHelper.Instance.GenerateRandomColor();
            Clients.Caller.receiveColor(color);
        }
        //not being used?
        public void NewFood()
        {
            lock (Foods)
            {
                for (int i = 0; i < 250; i++)
                {
                    Point foodP = new Point(Rng.Next(0, 2000), Rng.Next(0, 2000));
                    string color = RandomColor();
                    Food food = new Food() {Color = color, Position = foodP};
                    Foods.Add(food);
                    Clients.All.Food(food);
                }
            }
        }

        public void SendProjectileData(Projectile projectileData)
        {
            Projectile projectile = new Projectile(projectileData.Id, projectileData.OwnerId, projectileData.X, projectileData.Y, projectileData.DirectionX, projectileData.DirectionY, projectileData.Radius);

            Projectiles.Add(projectile);

            Clients.All.receiveProjectile(projectileData);
        }

        public void CheckCollisions()
        {
            //
        }

        public void SendDir(double dir)
        {
            lock (Sneks)
            {
                var snakeAggregate = new SnakeAggregate(Sneks);
                var snakeIterator = snakeAggregate.CreateIterator();

                while (snakeIterator.HasNext())
                {
                    var snek = snakeIterator.Next();
                    if (snek.ConnectionId.Equals(Context.ConnectionId))
                    {
                        snek.Dir = dir;
                        break;
                    }
                }
            }
        }

        //MEMENTO
        public void HandleEscapeKey()
        {
            lock (Sneks)
            {
                if (!isGameSaved)
                {
                    savedState = SaveGameState();
                    isGameSaved = true;
                    Clients.Caller.notifyChat("Game State Saved!");
                }
                else
                {
                    LoadGameState();
                    isGameSaved = false;
                    Clients.Caller.notifyChat("Game State Loaded!");
                }
            }
        }

        public GameStateMemento SaveGameState()
        {
            lock (Sneks)
            {
                Clients.Caller.notifyChat("Game state saved!");
                return new GameStateMemento(Sneks, Foods, Obstacles);
            }
        }

        public void LoadGameState()
        {
            if (savedState == null)
            {
                Clients.Caller.notifyChat("No saved game state found!");
                return;
            }

            lock (Sneks)
            {
                Sneks.Clear();
                Foods.Clear();
                Obstacles.Clear();
                Sneks.AddRange(savedState.GetSnakes());
                Foods.AddRange(savedState.GetFoods());
                Obstacles.AddRange(savedState.GetObstacles());
                Clients.All.AllPos(Sneks.SelectMany(s => s.Parts).ToList(), new Point(), Foods, Obstacles);
                Clients.Caller.notifyChat("Game State Loaded!");
            }
        }

        //PERFORMANCE TESTING
        public void TestPerformance()
        {
            try
            {
                Console.WriteLine("TestPerformance invoked");

                MeasurePerformance(TestWithoutFlyweight, "Without Flyweight");
                MeasurePerformance(TestWithFlyweight, "With Flyweight");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during TestPerformance: {ex.Message}");
            }
        }

        private void MeasurePerformance(Action testMethod, string testName)
        {
            try
            {
                // Force garbage collection to ensure clean memory state
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                long beforeMemory = GC.GetTotalMemory(true);
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Artificial load to simulate measurable execution time
                for (int i = 0; i < 100; i++) // Run the test 100 times
                {
                    testMethod.Invoke();
                }

                stopwatch.Stop();
                long afterMemory = GC.GetTotalMemory(true);

                var results = new
                {
                    TestName = testName,
                    ExecutionTime = stopwatch.ElapsedMilliseconds,
                    MemoryUsage = afterMemory - beforeMemory
                };

                Console.WriteLine($"Test: {results.TestName}, Execution Time: {results.ExecutionTime} ms, Memory Used: {results.MemoryUsage} bytes");

                // Send results back to the client
                Clients.Caller.notifyTestResults(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MeasurePerformance: {ex.Message}");
            }
        }


        private void TestWithoutFlyweight()
        {
            List<Food> foods = new List<Food>();
            Random rng = new Random();

            for (int i = 0; i < 10000; i++)
            {
                string color = $"#{rng.Next(0x1000000):X6}";
                Food newFood = new Food
                {
                    Color = color,
                    Position = new Point(rng.Next(0, 2000), rng.Next(0, 2000))
                };
                foods.Add(newFood);
            }
        }

        private static readonly string[] PredefinedColors = { "#FF0000", "#00FF00", "#0000FF", "#FFFF00", "#FF00FF" };
        private void TestWithFlyweight()
        {
            FoodFlyweightFactory foodFactory = new FoodFlyweightFactory();
            List<Food> foods = new List<Food>();
            Random rng = new Random();

            for (int i = 0; i < 10000; i++)
            {
                string color = PredefinedColors[rng.Next(PredefinedColors.Length)]; // Reuse colors
                Food sharedFood = foodFactory.GetFood(color);
                Food newFood = (Food)sharedFood.Clone();
                newFood.Position = new Point(rng.Next(0, 2000), rng.Next(0, 2000));
                foods.Add(newFood);
            }
        }
        //EO PERFORMANCE TESTING
    }
}