using RSPGame.Models;
using RSPGame.Models.Game;

namespace RSPGame.Services.Game
{
    public class RoundService
    {
        private readonly IRspService _rspService;

        private Round _round;

        public RoundService()
        {
            _rspService = new RspService();
            _round = new Round();
        }

        public bool CanPlay()
        {
            if (_round.Gamer1 == null || _round.Gamer2 == null)
            {
                return true;
            }

            return false;
        }

        public void AddGamerAction(GamerInfo gamer, GameActions action)
        {
            if (gamer == null)
                return;

            if (_round.Gamer1 == null)
            {
                _round.Gamer1 = gamer;
                _round.UserAction1 = action;
            }
            else if(_round.Gamer2 == null)
            {
                _round.Gamer2 = gamer;
                _round.UserAction2 = action;
            }
        }

        public Round GetRound()
        {
            return _round;
        }

        public Round GetCompleteRound()
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

            var round = new Round
            {
                Gamer1 = _round.Gamer1,
                Gamer2 = _round.Gamer2,
                UserAction1 = _round.UserAction1,
                UserAction2 = _round.UserAction2,
                RoundResultForGamer1 = _round.RoundResultForGamer1,
                RoundResultForGamer2 = _round.RoundResultForGamer2
            };
            

            return round;
        }

        public void RefreshRound()
        {
            _round = new Round();
        }
    }
}