using RSPGame.Models;
using System.Threading.Tasks;

namespace RSPGame.Services
{
    public interface IRoomService
    {
        public Task CreateRoom(GamerInfo gamer, RoomStatus roomStatus);
        public Task JoinRoom(GamerInfo gamer, int id = 0);
    }
}
