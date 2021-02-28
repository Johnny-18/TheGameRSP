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

        private static GameActions BotDecision()
        {
            var random = new Random();

            return random.Next(0, 3) switch
            {
                0 => GameActions.Paper,
                1 => GameActions.Rock,
                2 => GameActions.Scissors,
                _ => GameActions.None
            };
        }
    }
}