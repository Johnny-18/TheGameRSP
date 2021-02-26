using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.Models.RoomModel;

namespace RSPGame.Services.Rooms
{
    public interface IRoomService
    {
        RoomRepository GetRoomRepById(int id);
        Task<int> CreateRoomAsync(GamerInfo gamer, bool isPrivate);
        Task<int> JoinRoomAsync(GamerInfo gamer, int id = 0);

        bool RemoveRoom(int id);
    }
}
