using System;
using RSPGame.Models;
using RSPGame.Models.Game;

namespace RSPGame.Services.Game
{
    public class RoundService
    {
        private readonly IRspService _rspService;

        private Round _round;

        public RoundService(IRspService rspService)
        {
            _rspService = rspService;
            _round = new Round();
        }

        public bool CanPlay()
        {
            if (_round.Gamer1 != null && _round.Gamer2 != null)
            {
                return true;
            }

            return false;
        }

        public void AddGamerAction(GamerInfo gamer, GameActions action)
        {
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            if (_round.Gamer1 != null)
            {
                _round.Gamer1 = gamer;
                _round.UserAction1 = action;
            }
            else if(_round.Gamer2 != null)
            {
                _round.Gamer2 = gamer;
                _round.UserAction2 = action;
            }
        }

        public Round PlayRound()
        {
            _round.RoundResultForGamer1 = _rspService.GetWinner(_round.UserAction1, _round.UserAction2);
            if (_round.RoundResultForGamer1 == RoundResult.None)
            {
                return null;
            }

            _round.RoundResultForGamer2 = _rspService.GetWinner(_round.UserAction2, _round.UserAction1);
            if (_round.RoundResultForGamer2 == RoundResult.None)
            {
                return null;
            }

            return _round;
        }
    }
}