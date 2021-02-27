using System.Collections.Generic;
using System.Threading.Tasks;
using RSPGame.Models.GameModel;
using RSPGame.Models.RoomModel;

namespace RSPGame.Services.RoomService
{
    public interface IRoomService
    {
        public Task<int> CreateRoom(GamerInfo gamer, RoomStatus roomStatus);
        public Task<int> JoinRoom(GamerInfo gamer, int id = 0);
        public IEnumerable<GamerInfo> GetGamers(int id);
        public RoomRepository GetRoom(int id);
        public void DeleteRoom(int id);

    }
}
