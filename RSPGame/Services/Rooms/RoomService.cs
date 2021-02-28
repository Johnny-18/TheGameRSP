using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RSPGame.Models;
using RSPGame.Models.Game;
using RSPGame.Services.Statistics;
using RSPGame.Storage;

namespace RSPGame.Services.Rooms
{
    public class RoomService : IRoomService
    {
        private readonly RoomStorage _roomStorage;

        private readonly IRspStorage _rspStorage;

        private readonly IIndividualStatService _individualStat;
        
        private readonly ILogger<RoomService> _logger;

        private static readonly object Locker = new();

        public RoomService(RoomStorage roomStorage, ILogger<RoomService> logger, IRspStorage rspStorage)
        {
            _logger = logger;
            _rspStorage = rspStorage;
            _individualStat = new IndividualStatService();
            _roomStorage = roomStorage;
        }

        public RoomRepository GetRoomRepById(int id)
        {
            lock (Locker)
            {
                return _roomStorage.Rooms.FirstOrDefault(x => x.Id == id);
            }
        }

        public async Task<int> CreateRoomAsync(GamerInfo gamer, bool isPrivate)
        {
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));
            
            //create private room
            var roomRep = new RoomRepository(isPrivate);
            
            _logger.Log(LogLevel.Information, $"Create room with Id {roomRep.Id}");

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
            
            return roomRep.Id;
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
                    roomRep = _roomStorage.Rooms.FirstOrDefault(x => !x.IsPrivate && x.IsFree());
                    if (roomRep == null)
                    {
                        //create public room
                        roomRep = new RoomRepository(false);

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
                    roomRep = _roomStorage.Rooms.FirstOrDefault(x => x.Id == id && x.IsPrivate && x.IsFree());
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
            
            return roomRep.Id;
        }

        public async Task SaveStatForGamersAsync(int roomId)
        {
            var roomRep = GetRoomRepById(roomId);
            if(roomRep == null)
                return;

            var rounds = roomRep.SeriesRepository.GetRounds();

            foreach (var round in rounds)
            {
                await _individualStat.ChangeGamerInfoAfterRound(round.Gamer1, round.UserAction1, round.RoundResultForGamer1);
                await _individualStat.ChangeGamerInfoAfterRound(round.Gamer2, round.UserAction2, round.RoundResultForGamer2);
            }
            
            rounds.TryPeek(out Round result);
            if(result == null)
                return;

            var user1 = await _rspStorage.GetUserByUserNameAsync(result.Gamer1.UserName);
            var newUser1 = new User
            {
                UserName = user1.UserName,
                PasswordHash = user1.PasswordHash,
                GamerInfo = result.Gamer1
            };
            
            var user2 = await _rspStorage.GetUserByUserNameAsync(result.Gamer2.UserName);
            var newUser2 = new User
            {
                UserName = user2.UserName,
                PasswordHash = user2.PasswordHash,
                GamerInfo = result.Gamer2
            };

            _rspStorage.TryUpdate(user1.UserName, newUser1);
            _rspStorage.TryUpdate(user2.UserName, newUser2);

            await _rspStorage.SaveToFile();
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
                if (_roomStorage.Rooms.Exists(x => x.Id == roomRepository.Id))
                {
                    _roomStorage.Rooms.Remove(roomRepository);
                    return true;
                }

                return false;
            }
        }
    }
}
