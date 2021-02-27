namespace RSPGame.Models.Game
{
    public class Round
    {
        public GamerInfo Gamer1 { get; set; }
        
        public GameActions UserAction1 { get; set; }
        
        public RoundResult RoundResultForGamer1 { get; set; }
        
        public GamerInfo Gamer2 { get; set; }
        
        public GameActions UserAction2 { get; set; }
        
        public RoundResult RoundResultForGamer2 { get; set; }

        public bool IsValid() => RoundResultForGamer1 != RoundResult.None && RoundResultForGamer2 != RoundResult.None;
    }
}