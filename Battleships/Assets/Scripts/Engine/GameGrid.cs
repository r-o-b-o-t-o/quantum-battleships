using System;
using System.Collections.Generic;

namespace Engine
{
    [Serializable]
    public class Cell
    {
        public Coords pos;
        public bool ship;
        public int damage;
    }

    [Serializable]
    public class Grid
    {
        public List<Cell> cells;
    }
}
