using System;
using RSPGame.Models;

namespace RSPGame.Services
{
    public class RspService : IRspService
    {
        /// <summary>
        /// Method choose who is winner.
        /// </summary>
        /// <param name="gamer1">Action of first gamer.</param>
        /// <param name="gamer2">Action of second gamer.</param>
        /// <returns>1 if gamer1 is win, -1 if lose, 0 if draw.</returns>
        public int GetWinner(GameActions gamer1, GameActions gamer2)
        {
            if (gamer1 == GameActions.None || gamer2 == GameActions.None)
                throw new ArgumentException("Gamers need to do actions !");
            
            if (gamer1 == gamer2)
            {
                return 0;
            }

            if (gamer1 == GameActions.Paper && gamer2 == GameActions.Rock ||
                gamer1 == GameActions.Rock && gamer2 == GameActions.Scissors ||
                gamer1 == GameActions.Scissors && gamer2 == GameActions.Paper)
            {
                return 1;
            }
            
            return -1;
        }
    }
}