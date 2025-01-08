using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project8
{
    public class Point
    {
        public int x { get; set; }
        public int y { get; set; }

        //constructor for player move input
        public Point(string x, string y)
        {
            this.x = int.Parse(x);
            this.y = int.Parse(y);
        }

        //Constructor for move calculations
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

    }
}