using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RSPGame.Models;
using RSPGame.Models.RoomModel;
using RSPGame.Services.Game;
using RSPGame.Services.Statistics;
using RSPGame.Storage;

namespace RSPGame.Services.Rooms
{
    public class RoomService : IRoomService
    {
        //rooms with 1 free slot
        private readonly RoomStorage _roomStorage;

        private readonly IIndividualStatService _individualStat;
        
        private readonly ILogger<RoomService> _logger;

        private static readonly object Locker = new();

        public RoomService(RoomStorage roomStorage, ILogger<RoomService> logger)
        {
            _logger = logger;
            _individualStat = new IndividualStatService();
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
            var roomRep = new RoomRepository(new Room(isPrivate));
            
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

        public int JoinRoom(GamerInfo gamer, int id = 0)
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
                        //create public room
                        roomRep = new RoomRepository(new Room(false));

                        roomRep.AddGamer(gamer);

                        _roomStorage.Rooms.Add(roomRep);
                    }
                    else
                    {
                        roomRep.AddGamer(gamer);
                    }
                }
                else
                {
                    roomRep = _roomStorage.Rooms.FirstOrDefault(x => x.GetId() == id && x.IsPrivate() && x.IsFree());
                    if (roomRep == null)
                    {
                        throw new ArgumentNullException(nameof(roomRep), "No rooms with this id found!");
                    }

                    roomRep.AddGamer(gamer);
                }
            }
            finally
            {
                if (acquiredLock) 
                    Monitor.Exit(Locker);
            }
            
            return roomRep.GetId();
        }

        public void SaveStatForGamers(int roomId)
        {
            var roomRep = GetRoomRepById(roomId);
            if(roomRep == null)
                return;

            var rounds = roomRep.SeriesRepository.GetRounds();
            var gamers = roomRep.GetGamers().ToList();

            foreach (var round in rounds)
            {
                
            }
        }
        
        //private void 

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
