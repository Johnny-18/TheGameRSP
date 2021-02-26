using System;
using RSPGame.Models;
using RSPGame.Models.GameModel;
using RSPGame.Services.Rsp;

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

            return _service.GetWinner(action, botAction);
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