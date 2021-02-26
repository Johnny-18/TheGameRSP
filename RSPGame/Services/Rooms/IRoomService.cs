using System.Threading.Tasks;
using RSPGame.Models;

namespace RSPGame.Services.Rooms
{
    public interface IRoomService
    {
        RoomRepository GetRoomRepById(int id);
        Task<int> CreateRoomAsync(GamerInfo gamer, bool isPrivate);
        int JoinRoom(GamerInfo gamer, int id = 0);
        bool RemoveRoom(int id);
    }
}
