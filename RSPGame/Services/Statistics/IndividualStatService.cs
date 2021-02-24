using System;
using RSPGame.Models;

namespace RSPGame.Services.Statistics
{
    public class IndividualStatService : IIndividualStatService
    {
        public void ChangeGamerInfoAfterRound(GamerInfo gamerInfo, GameActions action, RoundResult status)
        {
            if (gamerInfo == null)
                throw new ArgumentNullException(nameof(gamerInfo));
            if (action == GameActions.None)
                throw new ArgumentException("Invalid action!");
            if (status == RoundResult.None)
                throw new ArgumentException("Invalid game status!");
            
            switch (action)
            {
                case GameActions.Paper:
                    gamerInfo.CountPapers++;
                    break;
                case GameActions.Rock:
                    gamerInfo.CountRocks++;
                    break;
                case GameActions.Scissors:
                    gamerInfo.CountScissors++;
                    break;
            }

            switch (status)
            {
                case RoundResult.Draw:
                    gamerInfo.CountDraws++;
                    break;
                case RoundResult.Lose:
                    gamerInfo.CountLoses++;
                    break;
                case RoundResult.Win:
                    gamerInfo.CountWins++;
                    break;
            }
        }

        public void ChangeOnlineTime(GamerInfo gamerInfo, TimeSpan onlineTime)
        {
            if (gamerInfo == null)
                throw new ArgumentNullException(nameof(gamerInfo));
            if (onlineTime == TimeSpan.Zero)
                throw new ArgumentException("Online time is zero!");

            gamerInfo.OnlineTime += onlineTime;
        }
    }
}