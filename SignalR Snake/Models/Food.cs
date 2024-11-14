using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using SignalR_Snake.Models.Prototype;

namespace SignalR_Snake.Models
{
    public class Food : IPrototype
    {
        public Point Position { get; set; }
        public string Color { get; set; }

        // Implementing Clone method from IPrototype interface
        public IPrototype Clone()
        {
            return (IPrototype)this.MemberwiseClone(); // Creates a shallow copy
        }
    }
}