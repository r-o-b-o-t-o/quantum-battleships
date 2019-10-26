namespace Engine
{
    public class Query
    {
        public GameState gameState;
        public uint shots;

        public Query(GameState gameState, uint shots)
        {
            this.gameState = gameState;
            this.shots = shots;
        }
    }
}
