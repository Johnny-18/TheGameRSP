using RSPGame.Models;

namespace RSPGame.Services
{
    public class RspService : IRspService
    {
        /// <summary>
        /// Method choose who is winner
        /// </summary>
        /// <param name="gamer1">Action of first gamer</param>
        /// <param name="gamer2">Action of second gamer</param>
        /// <returns>Round status for gamer1</returns>
        public RoundResult GetWinner(GameActions gamer1, GameActions gamer2)
        {
            if (gamer1 == GameActions.None || gamer2 == GameActions.None)
                return RoundResult.None;
            
            if (gamer1 == gamer2)
            {
                return RoundResult.Draw;
            }

            if (gamer1 == GameActions.Paper && gamer2 == GameActions.Rock ||
                gamer1 == GameActions.Rock && gamer2 == GameActions.Scissors ||
                gamer1 == GameActions.Scissors && gamer2 == GameActions.Paper)
            {
                return RoundResult.Win;
            }
            
            return RoundResult.Lose;
        }
    }
}