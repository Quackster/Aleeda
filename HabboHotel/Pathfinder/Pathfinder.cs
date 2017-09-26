using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aleeda.HabboHotel.Pathfinder
{
    /*struct Pathfinder : IPathfinder, IDisposable
    {
        public TileState[,] RoomTiles;

        internal short HighestX = new short();
        internal short HighestY = new short();

        internal Dictionary<Coord, Boolean> AttemptTiles = new Dictionary<Coord, Boolean>();

        public Pathfinder(TileState[,] RoomTiles)
        {
            this.RoomTiles = RoomTiles;
        }

        public void Prepare()
        {
            HighestX = (short)RoomTiles.GetUpperBound(0);
            HighestY = (short)RoomTiles.GetUpperBound(1);
        }

        public void PopTiles(Coord End)
        {
            for (int y = 0; y < HighestY; y++)
            {
                for (int x = 0; x < HighestX; x++)
                {
                    TileState T = RoomTiles[x, y];

                    if (T.Equals(TileState.Walkable) || (Coord.Compare(new Coord(x, y), End) && T.Equals(TileState.WalkableLaststep)))
                    {
                        AttemptTiles.Add(new Coord(x, y), true);
                    }
                    else
                    {
                        AttemptTiles.Add(new Coord(x, y), false);
                    }
                }
            }
        }

        public List<Coord> ReturnPath(Coord a, Coord b)
        {
        }
    }*/
}