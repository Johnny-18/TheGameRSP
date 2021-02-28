using System.Collections.Concurrent;
using RSPGame.Models.Game;

namespace RSPGame.Services.Game
{
    public class SeriesRepository 
    {
        public readonly ConcurrentStack<Round> _rounds;

        public SeriesRepository()
        {
            _rounds = new ConcurrentStack<Round>();
        }

        public void AddRound(Round round)
        {
            if (round == null) 
                return;

            _rounds.Push(round);
        }

        public Round RemoveLastRound()
        {
            _rounds.TryPop(out var round);

            return round;
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