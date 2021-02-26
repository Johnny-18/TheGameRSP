using System;
using System.Collections.Generic;
using RSPGame.Models.Game;

namespace RSPGame.Services.Game
{
    public class SeriesRepository 
    {
        private readonly Stack<Round> _rounds;

        public SeriesRepository()
        {
            _rounds = new Stack<Round>();
        }

        public void AddRound(Round round)
        {
            if (round == null)
                throw new ArgumentNullException(nameof(round));
            if (round.Gamer1 == null)
                throw new ArgumentException(nameof(round.Gamer1));
            if (round.Gamer2 == null)
                throw new ArgumentException(nameof(round.Gamer2));

            _rounds.Push(round);
        }

        public Round GetLastRound()
        {
            return _rounds.Peek();
        }

        public Stack<Round> GetRounds()
        {
            return _rounds ?? new Stack<Round>();
        }
    }
}