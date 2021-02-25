using RSPGame.Models;
using RSPGame.Services;
using Xunit;

namespace Tests
{
    public class RspServiceTests
    {
        private readonly IRspService _service;

        public RspServiceTests()
        {
            _service = new RspService();
        }

        [Theory]
        [InlineData(GameActions.Paper, GameActions.Rock)]
        [InlineData(GameActions.Rock, GameActions.Scissors)]
        [InlineData(GameActions.Scissors, GameActions.Paper)]
        public void GetWinner_MustReturnWin_IfFirstAction_Win_SecondAction(GameActions gamer1, GameActions gamer2)
        {
            Assert.Equal(RoundResult.Win, _service.GetWinner(gamer1, gamer2));
        }
        
        [Theory]
        [InlineData(GameActions.Scissors, GameActions.Rock)]
        [InlineData(GameActions.Paper, GameActions.Scissors)]
        [InlineData(GameActions.Rock, GameActions.Paper)]
        public void GetWinner_MustReturnLose_IfFirstAction_Lose_SecondAction(GameActions gamer1, GameActions gamer2)
        {
            Assert.Equal(RoundResult.Lose, _service.GetWinner(gamer1, gamer2));
        }
        
        [Theory]
        [InlineData(GameActions.Paper, GameActions.Paper)]
        [InlineData(GameActions.Rock, GameActions.Rock)]
        [InlineData(GameActions.Scissors, GameActions.Scissors)]
        public void GetWinner_MustReturnDraw_IfActionsIsEqual(GameActions gamer1, GameActions gamer2)
        {
            Assert.Equal(RoundResult.Draw, _service.GetWinner(gamer1, gamer2));
        }
    }
}