using System;
using System.Linq;
using System.Collections.Generic;

namespace Engine
{
    [Serializable]
    public class Ship
    {
        [Serializable]
        public class Block
        {
            public Coords pos;
            public int damage;

            public Block(Coords pos)
            {
                this.pos = pos;
                this.damage = 0;
            }

            public Block(UnityEngine.Vector2Int pos) : this((Coords)pos) { }
        }

        public List<Block> blocks;
        public int health;

        public Ship()
        {
            this.blocks = new List<Block>();
        }

        public bool IsSunk()
        {
            return this.blocks.All(block => block.damage > 95);
        }
    }
}
