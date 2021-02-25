using RSPGame.Models;

namespace RSPGame.Services
{
    public interface IRspService
    {
        RoundResult GetWinner(GameActions gamer1, GameActions gamer2);
    }
}