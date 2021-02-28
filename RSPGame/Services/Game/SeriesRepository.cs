using System.Collections.Concurrent;
using RSPGame.Models.Game;

namespace RSPGame.Services.Game
{
    public class SeriesRepository 
    {
        public readonly ConcurrentStack<Round> Rounds;

        public SeriesRepository()
        {
            Rounds = new ConcurrentStack<Round>();
        }

        public void AddRound(Round round)
        {
            if (round == null) 
                return;

            Rounds.Push(round);
        }

        public ConcurrentStack<Round> GetRounds()
        {
            return Rounds ?? new ConcurrentStack<Round>();
        }
    }
}