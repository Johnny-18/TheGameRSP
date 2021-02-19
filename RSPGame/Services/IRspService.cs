using RSPGame.Models;

namespace RSPGame.Services
{
    public interface IRspService
    {
        int GetWinner(GameActions gamer1, GameActions gamer2);
    }
}