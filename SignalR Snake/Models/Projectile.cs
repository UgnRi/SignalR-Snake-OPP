using System;
using SignalR_Snake.Models;

namespace SignalR_Snake.Models
{
    public class Projectile
    {
        public int Id { get; set; }
        public string OwnerId { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Radius { get; private set; } = 5; // Default radius
        public double Speed { get; private set; } = 5;
        public double DirectionX { get; private set; }
        public double DirectionY { get; private set; }
        //public bool IsActive { get; private set; }

        public Projectile(int id, string ownerId, double startX, double startY, double DirectionX, double DirectionY, /*bool isActive,*/ double radius = 5, double speed = 5)
        {
            Id = id;
            OwnerId = ownerId;
            X = startX;
            Y = startY;
            this.DirectionX = DirectionX;
            this.DirectionY = DirectionY;
            //IsActive = isActive;
            Radius = radius;
            Speed = speed;
        }

        // Update projectile position
        public void UpdatePosition()
        {
            X += DirectionX;
            Y += DirectionY;
        }

        // Check if the projectile is out of bounds (assuming bounds are defined)
        public bool IsOutOfBounds(double canvasWidth, double canvasHeight)
        {
            return X < 0 || X > canvasWidth || Y < 0 || Y > canvasHeight;
        }
    }
}
