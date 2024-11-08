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
using SignalR_Snake.Models.Builder;
using SignalR_Snake.Models.Factory;
using SignalR_Snake.Models.Strategies;
using SignalR_Snake.Models.Observer;
using SignalR_Snake.Utilities;
using Timer = System.Timers.Timer;

namespace SignalR_Snake.Hubs
{
    public class SnakeHub : Hub, ISnakeObserver
    {

        public static List<Snake> Sneks = new List<Snake>();
        public static List<Food> Foods = new List<Food>();
        private static IHubCallerConnectionContext<dynamic> clientsStatic;
        public static Random Rng = new Random();

        public List<ISnakeObserver> observers = new List<ISnakeObserver>();

        public SnakeHub()
        {
            var chatObserver = new ChatObserver();
            RegisterObserver(chatObserver);
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
        public void NewSnek(string name, string snakeType)
        {

            SnakeFactory snakeFactory;
            switch (snakeType.ToLower())
            {
                case "long":
                    snakeFactory = new LongSnakeFactory();
                    break;
                case "medium":
                    snakeFactory = new MediumSnakeFactory();
                    break;
                case "short":
                    snakeFactory = new ShortSnakeFactory();
                    break;
                case "basic":
                    snakeFactory = new BasicSnakeFactory();
                    break;
                default:
                    snakeFactory = new BasicSnakeFactory();
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
        
        private static Food PrototypeFood = new Food() { Color = RandomColor() };
        private static void MoveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (Sneks)
            {
                foreach (var snek in Sneks)
                {
                    //Strategy


                    Point nextPosition = snek.MovementStrategy.Move(snek.Parts[0].Position, snek.Dir, snek.Speed);

                    for (int i = 0; i < snek.Parts.Count - 1; i++)
                    {
                        snek.Parts[snek.Parts.Count - (i + 1)].Position = snek.Parts[snek.Parts.Count - (2 + i)].Position;
                    }

                    snek.Parts[0].Position = nextPosition;
                    lock (Foods)
                    {
                        for (int i = 0; i < 1000 - Foods.Count; i++)
                        {
                            Food clonedFood = PrototypeFood.Clone();
                            clonedFood.Position = new Point(Rng.Next(0, 2000), Rng.Next(0, 2000));
                            
                            clonedFood.Color = RandomColor();

                            Console.WriteLine($"Original Food Memory Address: {PrototypeFood.GetHashCode()}");
                            Console.WriteLine($"Cloned Food Memory Address: {clonedFood.GetHashCode()}");
                        
                            Foods.Add(clonedFood);
                        }

                        List<Food> toRemove = new List<Food>();
                        foreach (var food in Foods.ToList())
                        {
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
                }

            }
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (Sneks)
            {
                List<Snake> toRemoveSnakes = new List<Snake>();
                foreach (var snek in Sneks)
                {
                    if (!Sneks.Any(x => x.Parts.Any(c => c.Position.X > snek.Parts[0].Position.X - snek.Width &&
                                                         c.Position.X < snek.Parts[0].Position.X + snek.Width
                                                         && c.Position.Y > snek.Parts[0].Position.Y - snek.Width
                                                         && c.Position.Y < snek.Parts[0].Position.Y + snek.Width) &&
                                        snek != x)) return;

                    clientsStatic.User(snek.ConnectionId).Died();
                    foreach (var part in snek.Parts)
                    {
                        Foods.Add(new Food() {Color = RandomColor(), Position = part.Position});
                    }
                    toRemoveSnakes.Add(snek);
                    clientsStatic.User(snek.ConnectionId).Died();
                    break;
                }
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
                Clients.Caller.AllPos(snakeParts, myPoint, Foods);
                //Clients.All.AllPos(snakePoints.ToArray(), myPoint);
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

        public void CheckCollisions()
        {
            //
        }

        public void SendDir(double dir)
        {
            lock (Sneks)
            {
                foreach (var snek in Sneks.Where(snek => snek.ConnectionId.Equals(Context.ConnectionId)))
                {
                    snek.Dir = dir;
                }
            }

            #region 

/*
            lock (Sneks)
            {
                if (!Sneks.Any(x => x.ConnectionId.Equals(Context.ConnectionId))) return;
                Snake snek = Sneks.First(x => x.ConnectionId.Equals(Context.ConnectionId));
                Point nextPosition;
                if (snek.Fast)
                {
                    nextPosition =
                        new Point(snek.Parts[0].Position.X + (int) (Math.Cos(dir*(Math.PI/180))*snek.SpeedTwo),
                            snek.Parts[0].Position.Y + (int) (Math.Sin(dir*(Math.PI/180))*snek.SpeedTwo));
                }
                else
                {
                    nextPosition = new Point(snek.Parts[0].Position.X + (int) (Math.Cos(dir*(Math.PI/180))*snek.Speed),
                        snek.Parts[0].Position.Y + (int) (Math.Sin(dir*(Math.PI/180))*snek.Speed));
                }


                for (int i = 0; i < snek.Parts.Count - 1; i++)
                {
                    if (i != snek.Parts.Count - 1)
                    {
                        snek.Parts[snek.Parts.Count - (i + 1)].Position =
                            snek.Parts[snek.Parts.Count - (2 + i)].Position;
                    }
                }
                snek.Parts[0].Position = nextPosition;
                //snek.Position[0].X += (int)(Math.Cos(dir * (Math.PI / 180)) * snek.Speed);
                //snek.Position[0].Y += (int)(Math.Sin(dir * (Math.PI / 180)) * snek.Speed);
                /*

            List<Point> snakePoints = new List<Point>();

            foreach (var snake in Sneks)
            {
                for (int i = 0; i < snake.Position.Count; i++)
                {
                    snakePoints.Add(snake.Position[i]);
                }
            }
            
                lock (Foods)
                {
                    if (Foods.Count < 1000)
                    {
                        for (int i = 0; i < 1000 - Foods.Count; i++)
                        {
                            Point foodP = new Point(Rng.Next(0, 2000), Rng.Next(0, 2000));
                            Food food = new Food() {Color = RandomColor(), Position = foodP};
                            Foods.Add(food);
                            //Foods.Add(new Point(rng.Next(0,1000),rng.Next(0,1000)));
                        }
                    }

                    List<Food> toRemove = new List<Food>();
                    foreach (var food in Foods.ToList())
                    {
                        int w = snek.Width;
                        if (snek.Parts[0].Position.X - w <= food.Position.X &&
                            snek.Parts[0].Position.X + w >= food.Position.X &&
                            snek.Parts[0].Position.Y - w < food.Position.Y &&
                            snek.Parts[0].Position.Y + 2*w > food.Position.Y)
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
                */
            //Clients.Client(Context.ConnectionId).Pos(snakePoints.ToArray(), snek.Position[0]);}

            #endregion
        }
    }
}