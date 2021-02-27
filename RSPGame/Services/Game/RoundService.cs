using System;
using RSPGame.Models.Game;
using RSPGame.Models.GameModel;
using RSPGame.Services.Rsp;

namespace RSPGame.Services.Game
{
    public class RoundService
    {
        private readonly IRspService _rspService;

        public RoundService(IRspService rspService)
        {
            _rspService = rspService;
        }

        public Round PlayRound(GamerInfo gamer1, GameActions action1, GamerInfo gamer2, GameActions action2)
        {
            if (gamer1 == null)
                throw new ArgumentNullException(nameof(gamer1));
            if (gamer2 == null)
                throw new ArgumentNullException(nameof(gamer2));
            if (action1 == GameActions.None || action2 == GameActions.None)
                throw new ArgumentException("Invalid action!");

            var result1 = _rspService.GetWinner(action1, action2);
            var result2 = _rspService.GetWinner(action2, action1);

            return new Round
            {
                Gamer1 = gamer1,
                Gamer2 = gamer2,
                RoundResultForGamer1 = result1,
                RoundResultForGamer2 = result2,
                UserAction1 = action1,
                UserAction2 = action2
            };
        }
    }
}