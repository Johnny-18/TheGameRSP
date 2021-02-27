using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RSPGame.Models.GameModel;
using RSPGame.Models.RoomModel;
using RSPGame.Storage;

namespace RSPGame.Services.RoomService
{
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

            var room = new RoomRepository(roomStatus);

            _logger.LogInformation($"Create room with Id {room.GetId()}");

            await room.AddGamer(gamer);

            var acquiredLock = false;
            try
            {
                Monitor.Enter(Locker, ref acquiredLock);
                _roomStorage.AddRoom(room);
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
                return -1;

            var acquiredLock = false;
            try
            {
                Monitor.Enter(Locker, ref acquiredLock);

                RoomRepository roomRepository;
                if (id == 0)
                {
                    roomRepository = _roomStorage.GetRooms()
                        .FirstOrDefault(x => x.IsPublic() && x.GetGamer().UserName != gamer.UserName);

                    if (roomRepository == null)
                    {
                        roomRepository = new RoomRepository(RoomStatus.Public);

                        _logger.LogInformation($"Create roomRepository with Id {roomRepository.GetId()}");

                        await roomRepository.AddGamer(gamer);

                        _roomStorage.AddRoom(roomRepository);
                        return roomRepository.GetId();
                    }
                }
                else
                {
                    roomRepository = _roomStorage.GetRooms()
                        .FirstOrDefault(x => x.GetId() == id && !x.IsPublic() && x.GetGamer().UserName != gamer.UserName);

                    if (roomRepository == null)
                    {
                        return 0;
                    }
                }

                await roomRepository.AddGamer(gamer);
                return roomRepository.GetId();

            }
            finally
            {
                if (acquiredLock) Monitor.Exit(Locker);
            }

        }

        public IEnumerable<GamerInfo> GetGamers(int id) => _roomStorage.GetRooms().Select(x => x.GetGamer());

        public RoomRepository GetRoom(int id) => _roomStorage.GetRooms().FirstOrDefault(x => x.GetId() == id);

        public void DeleteRoom(int id)
        {
           var room =_roomStorage.GetRooms().FirstOrDefault(x => x.GetId() == id);
            _roomStorage.RemoveRoom(room);
        }
    }
}
