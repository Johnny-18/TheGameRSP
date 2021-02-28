using System;
using System.Threading.Tasks;
using RSPGame.Models;

namespace RSPGame.Services.Statistics
{
    public class IndividualStatService : IIndividualStatService
    {
        public async Task ChangeGamerInfoAfterRound(GamerInfo gamerInfo, GameActions action, RoundResult status)
        {
            if (gamerInfo == null)
                return;
            if (action == GameActions.None)
                return;
            if (status == RoundResult.None)
                return;

            await Task.Run(() =>
            {
                ChangeCountAction(gamerInfo, action);
                ChangeCountStatus(gamerInfo, status);
            });
        }

        private void ChangeCountAction(GamerInfo gamerInfo, GameActions action)
        {
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
        }

        private void ChangeCountStatus(GamerInfo gamerInfo, RoundResult status)
        {
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
                return;
            if (onlineTime == TimeSpan.Zero)
                return;

            gamerInfo.OnlineTime += onlineTime;
        }
    }
}