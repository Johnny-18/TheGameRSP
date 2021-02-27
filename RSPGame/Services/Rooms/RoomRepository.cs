using System;
using System.Collections.Generic;
using RSPGame.Models;
using RSPGame.Models.Game;
using RSPGame.Models.RoomModel;
using RSPGame.Services.Game;

namespace RSPGame.Services.Rooms
{
    public class RoomRepository
    {
        public RoomRepository(Room room)
        {
            _room = room;
            SeriesRepository = new SeriesRepository();
            _currentRound = new RoundRepository();
        }

        public readonly SeriesRepository SeriesRepository;

        private readonly RoundRepository _currentRound;
        
        private readonly Room _room;

        private readonly object _locker = new();

        public int GetId() => _room.Id;

        public IEnumerable<GamerInfo> GetGamers() => _room.Gamers;

        public bool IsPrivate() => _room.IsPrivate;

        public bool IsFree()
        {
            if (_room == null || _room.Gamers == null)
                return false;
            
            return _room.Gamers.Count != 2;
        }

        public void AddGamer(GamerInfo gamer)
        {
            if (gamer == null)
                return;
            
            lock (_locker)
            {
                _room.Gamers.Add(gamer);
            }
        }
        
        public void AddGamerAction(GamerInfo gamer, GameActions action)
        {
            if (gamer == null)
                return;

            lock (_locker)
            {
                if (_currentRound.CanPlay() == false)
                    throw new InvalidOperationException("Two actions was done!");
                
                _currentRound.AddGamerAction(gamer, action);
            }
        }

        public Round TryGetCompleteRound()
        {
            if(_currentRound.CanPlay() == false)
                return _currentRound.GetCompleteRound();

            return null;
        }

        public Round GetCurrentRound()
        {
            return _currentRound.GetRound();
        }

        public void RefreshCurrentRound()
        {
            _currentRound.RefreshRound();
        }
    }
}