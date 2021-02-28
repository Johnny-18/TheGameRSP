using System;
using System.Collections.Generic;
using System.Linq;
using RSPGame.Models;
using RSPGame.Models.Game;
using RSPGame.Services.Game;

namespace RSPGame.Services.Rooms
{
    public class RoomRepository
    {
        public RoomRepository()
        {
            DefaultInitialize();
        }
        
        public RoomRepository(bool isPrivate)
        {
            DefaultInitialize();
            
            IsPrivate = isPrivate;
        }
        
        private void DefaultInitialize()
        {
            Id = _id;
            _id++;
            
            Gamers = new List<GamerInfo>(2);
            SeriesRepository = new SeriesRepository();
            _currentRound = new RoundRepository();
        }

        public SeriesRepository SeriesRepository;

        private RoundRepository _currentRound;
        
        private static int _id;

        static RoomRepository()
        {
            _id = 1;
        }
        
        public int Id { get; set; }

        private List<GamerInfo> Gamers { get; set; }

        public int ReadyCounter { get; private set; }

        public bool IsPrivate { get; }

        private readonly object _locker = new();
        
        public List<GamerInfo> GetGamers() => Gamers.ToList();

        public bool IsFree()
        {
            if (Gamers == null)
                return false;
            
            return Gamers.Count != 2;
        }

        public void SetReady()
        {
            if (ReadyCounter == 2)
            {
                ReadyCounter = 0;
            }
            
            ReadyCounter++;
        }

        public void AddGamer(GamerInfo gamer)
        {
            if (gamer == null)
                return;
            
            lock (_locker)
            {
                Gamers.Add(gamer);
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