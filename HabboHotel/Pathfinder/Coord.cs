using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aleeda.HabboHotel.Pathfinder
{
    struct Coord
    {
        public int X;
        public int Y;

        public Coord(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static Boolean Compare(Coord a, Coord b)
        {
            return a.X.Equals(b.X) && a.Y.Equals(b.Y);
        }
    }
}
