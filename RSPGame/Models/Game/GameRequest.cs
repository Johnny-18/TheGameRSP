namespace RSPGame.Models.Game
{
    public class GameRequest
    {
        public GamerInfo GamerInfo { get; set; }
        
        public GameActions Action { get; set; }
    }
}