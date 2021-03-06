﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RSPGame.Models.GameModel;

namespace RSPGame.Models.RoomModel
{
    public class RoomRepository
    {
        private readonly int _id = new Random().Next(1, 1001);

        private readonly RoomStatus _roomStatus;

        private readonly BlockingCollection<GamerInfo> _gamers;

        private readonly object _locker = new();

        public RoomRepository(RoomStatus roomStatus)
        {
            _roomStatus = roomStatus;
            _gamers = new BlockingCollection<GamerInfo>(2);
        }

        public Task AddGamer(GamerInfo gamer)
        {
            lock (_locker)
            {
                if (gamer == null)
                    throw new ArgumentNullException(nameof(gamer));

                _gamers.Add(gamer);
            }
            return Task.CompletedTask;
        }

        public int GetId() => _id;

        public GamerInfo GetGamer() => _gamers.FirstOrDefault();

        public IEnumerable<GamerInfo> GetGamers => _gamers;

        public bool IsPublic() => _roomStatus == RoomStatus.Public;

        public bool IsFree() => _gamers.Count != 2;
    }
}
