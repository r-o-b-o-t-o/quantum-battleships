using System;

namespace Engine
{
    [Serializable]
    public class Coords
    {
        public int x;
        public int y;

        public Coords(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Coords(UnityEngine.Vector2Int v)
        {
            this.x = v.x;
            this.y = v.y;
        }

        public static implicit operator Coords(UnityEngine.Vector2Int v)
        {
            return new Coords(v);
        }
    }
}
