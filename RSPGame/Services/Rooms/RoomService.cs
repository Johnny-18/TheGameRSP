using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RSPGame.Models;
using RSPGame.Models.RoomModel;
using RSPGame.Services.Game;
using RSPGame.Storage;

namespace RSPGame.Services.Rooms
{
    public class RoomService : IRoomService
    {
        //rooms with 1 free slot
        private readonly RoomStorage _roomStorage;

        private readonly RoundService _roundService;

        private readonly ILogger<RoomService> _logger;

        private static readonly object Locker = new();

        public RoomService(RoomStorage roomStorage, ILogger<RoomService> logger, RoundService roundService)
        {
            _logger = logger;
            _roundService = roundService;
            _roomStorage = roomStorage;
        }

        public RoomRepository GetRoomRepById(int id)
        {
            lock (Locker)
            {
                return _roomStorage.Rooms.FirstOrDefault(x => x.GetId() == id);
            }
        }

        public async Task<int> CreateRoomAsync(GamerInfo gamer, bool isPrivate)
        {
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));
            
            //create private room
            var roomRep = new RoomRepository(new Room(isPrivate), new SeriesRepository(), _roundService);
            
            _logger.Log(LogLevel.Information, $"Create room with Id {roomRep.GetId()}");

            await Task.Run(() => roomRep.AddGamer(gamer));

            var acquiredLock = false;
            try
            {
                Monitor.Enter(Locker, ref acquiredLock);
                _roomStorage.Rooms.Add(roomRep);
            }
            finally
            {
                if (acquiredLock) 
                    Monitor.Exit(Locker);
            }
            
            return roomRep.GetId();
        }

        public int JoinRoomAsync(GamerInfo gamer, int id = 0)
        {
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var acquiredLock = false;
            RoomRepository roomRep;
            try
            {
                Monitor.Enter(Locker, ref acquiredLock);

                if (id == 0)
                {
                    roomRep = _roomStorage.Rooms.FirstOrDefault(x => !x.IsPrivate() && x.IsFree());
                    if (roomRep == null)
                    {
                        roomRep = new RoomRepository(new Room(false), new SeriesRepository(), _roundService);

                        roomRep.AddGamer(gamer);

                        _roomStorage.Rooms.Add(roomRep);
                    }
                    else
                    {
                        RemoveRoomFromStorage(roomRep, gamer);
                    }
                }
                else
                {
                    roomRep = _roomStorage.Rooms.FirstOrDefault(x => x.GetId() == id && x.IsPrivate());
                    if (roomRep == null)
                    {
                        throw new ArgumentNullException(nameof(roomRep), "No rooms with this id found!");
                    }

                    RemoveRoomFromStorage(roomRep, gamer);
                }
            }
            finally
            {
                if (acquiredLock) 
                    Monitor.Exit(Locker);
            }
            
            return roomRep.GetId();
        }

        private void RemoveRoomFromStorage(RoomRepository roomRep, GamerInfo gamer)
        {
            roomRep.AddGamer(gamer);

            _roomStorage.Rooms.Remove(roomRep);
        }

        public bool RemoveRoom(int id)
        {
            var roomRepository = GetRoomRepById(id);
            return RemoveRoom(roomRepository);
        }

        private bool RemoveRoom(RoomRepository roomRepository)
        {
            lock (Locker)
            {
                if (_roomStorage.Rooms.Exists(x => x.GetId() == roomRepository.GetId()))
                {
                    _roomStorage.Rooms.Remove(roomRepository);
                    return true;
                }

                return false;
            }
        }
    }
}
