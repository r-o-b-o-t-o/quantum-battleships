using System;
using System.Linq;
using System.Collections.Generic;

namespace Engine
{
    [Serializable]
    public class Player
    {
        public List<Ship> ships;
        public List<Coords> bombs;
        public Grid grid;

        public Player()
        {
            this.ships = new List<Ship>();
            this.bombs = new List<Coords>();
        }

        public int CountSunkShips()
        {
            return this.ships.Where(s => s.IsSunk()).Count();
        }
    }
}
