using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.Services;

namespace RSPGame.Storage
{
    //todo: game logic
    //todo: statistics 
    //todo: post step on server
    //todo: get round result by gamers 
    //todo: get match result by gamers 
    public class RoomService : IRoomService
    {
        private readonly RoomStorage _roomStorage;

        private static readonly object Locker = new();

        public RoomService(RoomStorage roomStorage)
        {
            _roomStorage = roomStorage;
        }

        public async Task CreateRoom(GamerInfo gamer, RoomStatus roomStatus)
        {
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var room = new Room(roomStatus);
            await room.AddGamer(gamer);

            //Console.WriteLine("room`s id:\t" + room.GetId());

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

        }

        public async Task JoinRoom(GamerInfo gamer, int id = 0)
        {
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var acquiredLock = false;
            try
            {
                Monitor.Enter(Locker, ref acquiredLock);

                Room room;
                if (id == 0)
                {
                    room = _roomStorage.ListRooms
                        .FirstOrDefault(x => x.IsPublic());

                    if (room == null)
                    {
                        room = new Room(RoomStatus.Public);
                        await room.AddGamer(gamer);

                        _roomStorage.ListRooms.Add(room);
                        return;
                    }
                }
                else
                {
                    room = _roomStorage.ListRooms
                        .FirstOrDefault(x => x.GetId() == id && x.IsPublic());

                    if (room == null)
                    {
                        throw new ArgumentNullException(nameof(room), "No rooms with this id found!");
                    }
                }

                await room.AddGamer(gamer);

                _roomStorage.ListRooms.Remove(room);

            }
            finally
            {
                if (acquiredLock) Monitor.Exit(Locker);
            }

        }
    }
}
