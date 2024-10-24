using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace SignalR_Snake.Models
{
    public class SnekPart
    {
        public Point Position { get; set; }
        public string Color { get; set; }
        public string Name { get; set; } 
    }
}