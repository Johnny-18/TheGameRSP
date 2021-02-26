using System;
using System.Collections.Generic;
using RSPGame.Models.Game;
using RSPGame.Services.Statistics;
using RSPGame.Storage;

namespace RSPGame.Services.Game
{
    public class SeriesService : IDisposable
    {
        private readonly List<Round> _rounds;
        
        private bool _disposed = false;

        private readonly IIndividualStatService _individualStatService;

        private readonly IRspStorage _storage;

        public SeriesService(IIndividualStatService individualStatService, IRspStorage storage)
        {
            _individualStatService = individualStatService;
            _storage = storage;
            _rounds = new List<Round>();
        }

        public void AddRound(Round round)
        {
            if (round == null)
                throw new ArgumentNullException(nameof(round));
            if (round.Gamer1 == null)
                throw new ArgumentException(nameof(round.Gamer1));
            if (round.Gamer2 == null)
                throw new ArgumentException(nameof(round.Gamer2));

            _rounds.Add(round);
        }

        public List<Round> GetRounds()
        {
            return _rounds ?? new List<Round>();
        }
        
        public void Dispose() => Dispose(true);

        protected virtual async void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            foreach (var round in _rounds)
            {
                await _individualStatService.ChangeGamerInfoAfterRound(round.Gamer1, round.UserAction1,
                    round.RoundResultForGamer1);
                
                await _individualStatService.ChangeGamerInfoAfterRound(round.Gamer2, round.UserAction2,
                    round.RoundResultForGamer2);
            }

            _disposed = true;

            await _storage.SaveToFile();
        }
    }
}