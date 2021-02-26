using System;
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
            RoundService = new RoundService();
        }

        public readonly SeriesRepository SeriesRepository;

        public readonly RoundService RoundService;

        public bool IsStarted = false;
        
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

        // public Task GamersCheck()
        // {
        //     return Task.Run(async() =>
        //     {
        //         while (true)
        //         {
        //             if (_room.Gamers.Count != 2) continue;
        //             //request
        //             //request from controller
        //             //request
        //             await StartGame();
        //             break;
        //         }
        //     });
        // }

        public void AddGamer(GamerInfo gamer)
        {
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));
            
            lock (_locker)
            {
                _room.Gamers.Add(gamer);
            }

            if (_room.Gamers.Count == 2)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            IsStarted = true;
        }
    }
}