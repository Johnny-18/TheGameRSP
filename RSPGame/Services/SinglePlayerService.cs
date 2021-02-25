using System;
using RSPGame.Models;

namespace RSPGame.Services
{
    public class SinglePlayerService
    {
        private readonly IRspService _service;

        public SinglePlayerService(IRspService service)
        {
            _service = service;
        }

        public RoundResult PlayWithBot(GameActions action)
        {
            var botAction = BotDecision();

            var result = _service.GetWinner(action, botAction);
            switch (result)
            {
                case -1:
                    return RoundResult.Lose;
                case 0:
                    return RoundResult.Draw;
                case 1:
                    return RoundResult.Win;
                default:
                    return RoundResult.None;
            }
        }

        private GameActions BotDecision()
        {
            var random = new Random();

            switch (random.Next(0, 3))
            {
                case 0:
                    return GameActions.Paper;
                case 1:
                    return GameActions.Rock;
                case 2:
                    return GameActions.Scissors;
                default:
                    return GameActions.None;
            }
        }
    }
}