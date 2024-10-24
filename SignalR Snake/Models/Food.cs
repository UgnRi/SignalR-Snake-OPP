using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace SignalR_Snake.Models
{
    public class Food
    {
        //public int Width { get; set; }
        public Point Position { get; set; }
        public string Color { get; set; }
    }
}