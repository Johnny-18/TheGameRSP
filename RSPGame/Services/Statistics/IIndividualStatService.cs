using System;
using System.Threading.Tasks;
using RSPGame.Models;

namespace RSPGame.Services.Statistics
{
    public interface IIndividualStatService
    {
        Task ChangeGamerInfoAfterRound(GamerInfo gamerInfo, GameActions action, RoundResult status);

        void ChangeOnlineTime(GamerInfo gamerInfo, TimeSpan onlineTime);
    }
}