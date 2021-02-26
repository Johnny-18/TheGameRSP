using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RSPGame.Models;
using RSPGame.Storage;

namespace RSPGame.Services.Room
{
    //todo: game logic
    //todo: statistics 
    //todo: post step on server
    //todo: get round result by gamers 
    //todo: get match result by gamers 
    public class RoomService : IRoomService
    {
        private readonly RoomStorage _roomStorage;
        private readonly ILogger<RoomService> _logger;

        private static readonly object Locker = new();

        public RoomService(RoomStorage roomStorage, ILogger<RoomService> logger)
        {
            _roomStorage = roomStorage;
            _logger = logger;
        }

        public async Task<int> CreateRoom(GamerInfo gamer, RoomStatus roomStatus)
        {
            if (gamer == null)
                return -1;

            var room = new Models.Room(roomStatus);

            _logger.LogInformation($"Create room with Id {room.GetId()}");

            await room.AddGamer(gamer);

            var acquiredLock = false;
            try
            {
                Monitor.Enter(Locker, ref acquiredLock);
                _roomStorage.ListRooms.Add(room);
            }
            finally
            {
                if (acquiredLock) Monitor.Exit(Locker);
                
            }
            return room.GetId();
        }

        public async Task<int> JoinRoom(GamerInfo gamer, int id = 0)
        {
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var acquiredLock = false;
            try
            {
                Monitor.Enter(Locker, ref acquiredLock);

                Models.Room room;
                if (id == 0)
                {
                    room = _roomStorage.ListRooms
                        .FirstOrDefault(x => x.IsPublic());

                    if (room == null)
                    {
                        room = new Models.Room(RoomStatus.Public);

                        _logger.LogInformation($"Create room with Id {room.GetId()}");

                        await room.AddGamer(gamer);

                        _roomStorage.ListRooms.Add(room);
                        return room.GetId();
                    }
                }
                else
                {
                    room = _roomStorage.ListRooms
                        .FirstOrDefault(x => x.GetId() == id && !x.IsPublic());

                    if (room == null)
                    {
                        throw new ArgumentNullException(nameof(room), "No rooms with this id found!");
                    }
                }

                await room.AddGamer(gamer);

                _roomStorage.ListRooms.Remove(room);

                return room.GetId();

            }
            finally
            {
                if (acquiredLock) Monitor.Exit(Locker);
            }

        }
    }
}
