using System.Collections.Generic;
using RSPGame.Models;
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
            _roundService = new RoundService();
        }

        public readonly SeriesRepository SeriesRepository;

        private readonly RoundService _roundService;

        public bool IsStarted;
        
        private readonly Room _room;

        private readonly object _locker = new();

        public int GetId() => _room.Id;

        public IEnumerable<GamerInfo> GetGamers() => _room.Gamers;

        public bool IsFree()
        {
            if (_room.Gamers.Count == 2)
                return false;

            return true;
        }

        public bool IsPrivate() => _room.IsPrivate;

        public void AddGamer(GamerInfo gamer)
        {
            if (gamer == null)
                return;
            
            lock (_locker)
            {
                _room.Gamers.Add(gamer);
            }

            if (_room.Gamers.Count == 2)
            {
                StartGame();
                SeriesRepository.AddRound(_roundService.GetRound());
            }
        }
        
        public void AddGamerAction(GamerInfo gamer, GameActions action)
        {
            if (gamer == null)
                return;

            lock (_locker)
            {
                _roundService.AddGamerAction(gamer, action);

                //first action
                if (_roundService.CanPlay() == false)
                {
                    SeriesRepository.RemoveLastRound();
                    SeriesRepository.AddRound(_roundService.GetCompleteRound());
                    
                    _roundService.RefreshRound();
                }
            }
        }

        private void StartGame()
        {
            IsStarted = true;
        }
    }
}