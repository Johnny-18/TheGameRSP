using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.Models.RoomModel;

namespace RSPGame.Services.Room
{
    public interface IRoomService
    {
        public Task<int> CreateRoom(GamerInfo gamer, RoomStatus roomStatus);
        public Task<int> JoinRoom(GamerInfo gamer, int id = 0);
    }
}
