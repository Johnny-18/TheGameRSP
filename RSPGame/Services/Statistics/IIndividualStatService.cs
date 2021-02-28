using System;
using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.Models.GameModel;

namespace RSPGame.Services.Statistics
{
    public interface IIndividualStatService
    {
        Task ChangeGamerInfoAfterRound(GamerInfo gamerInfo, GameActions action, RoundResult status);

        GamerInfo ChangeGamerInfoAfterRound(GamerInfo gamerInfo, GamerInfo gamerNewInfo);

        void ChangeOnlineTime(GamerInfo gamerInfo, TimeSpan onlineTime);
    }
}