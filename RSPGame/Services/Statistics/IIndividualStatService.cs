using RSPGame.Models;

namespace RSPGame.Services.Statistics
{
    public interface IIndividualStatService
    {
        void ChangeGamerInfoAfterRound(GamerInfo gamerInfo, GameActions action, RoundResult status);
    }
}