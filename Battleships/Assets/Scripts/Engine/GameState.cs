using System;

namespace Engine
{
    [Serializable]
    public class GameState
    {
        public Player[] players;
        public int winner;
    }
}
