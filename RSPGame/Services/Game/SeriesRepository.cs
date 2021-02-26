using System.Collections.Concurrent;
using RSPGame.Models.Game;

namespace RSPGame.Services.Game
{
    public class SeriesRepository 
    {
        private readonly ConcurrentStack<Round> _rounds;

        public SeriesRepository()
        {
            _rounds = new ConcurrentStack<Round>();
        }

        public void AddRound(Round round)
        {
            if (round == null || round.Gamer1 == null || round.Gamer2 == null) 
                return;

            _rounds.Push(round);
        }

        public Round GetLastRound()
        {
            _rounds.TryPeek(out var round);

            return round;
        }

        public ConcurrentStack<Round> GetRounds()
        {
            return _rounds ?? new ConcurrentStack<Round>();
        }
    }
}