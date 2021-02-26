using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RSPGame.Models;
using RSPGame.Models.RoomModel;
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
        private readonly RoomStorage RoomStorage;
        private readonly ILogger<RoomService> _logger;

        private static readonly object Locker = new();

        public RoomService(RoomStorage roomStorage, ILogger<RoomService> logger)
        {
            RoomStorage = roomStorage;
            _logger = logger;
        }

        public async Task<int> CreateRoom(GamerInfo gamer, RoomStatus roomStatus)
        {
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var room = new Models.RoomModel.Room(roomStatus);

            _logger.LogInformation($"Create room with Id {room.GetId()}");

            await room.AddGamer(gamer);

            var acquiredLock = false;
            try
            {
                Monitor.Enter(Locker, ref acquiredLock);
                RoomStorage.ListRooms.Add(room);
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

                Models.RoomModel.Room room;
                if (id == 0)
                {
                    room = RoomStorage.ListRooms
                        .FirstOrDefault(x => x.IsPublic() /*&& x.GetGamer().UserName != gamer.UserName*/);

                    if (room == null)
                    {
                        room = new Models.RoomModel.Room(RoomStatus.Public);

                        _logger.LogInformation($"Create room with Id {room.GetId()}");

                        await room.AddGamer(gamer);

                        RoomStorage.ListRooms.Add(room);
                        return room.GetId();
                    }
                }
                else
                {
                    room = RoomStorage.ListRooms
                        .FirstOrDefault(x => x.GetId() == id && !x.IsPublic() /*&& x.GetGamer().UserName != gamer.UserName*/);

                    if (room == null)
                    {
                        throw new ArgumentNullException(nameof(room), "No rooms with this id found!");
                    }
                }

                await room.AddGamer(gamer);

                RoomStorage.ListRooms.Remove(room);

                return room.GetId();

            }
            finally
            {
                if (acquiredLock) Monitor.Exit(Locker);
            }

        }

        public Task DeleteRoom(int id)
        {
            var room =RoomStorage.ListRooms.FirstOrDefault(x => x.GetId() == id);
            RoomStorage.ListRooms.Remove(room);

            return Task.CompletedTask;
        }
    }
}
