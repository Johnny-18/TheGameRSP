using RSPGame.Models;
using RSPGame.Models.GameModel;

namespace RSPGame.Services.Rsp
{
    public interface IRspService
    {
        RoundResult GetWinner(GameActions gamer1, GameActions gamer2); 
        RoundResult InverseResult(RoundResult result);
    }
}