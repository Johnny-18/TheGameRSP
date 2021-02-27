using System.Collections.Generic;
using RSPGame.Models.RoomModel;

namespace RSPGame.Storage
{
    public class RoomStorage
    {
        private readonly List<RoomRepository> _listRooms = new();

        public void AddRoom(RoomRepository roomRepository) => _listRooms.Add(roomRepository);

        public void RemoveRoom(RoomRepository roomRepository) => _listRooms.Remove(roomRepository);

        public IEnumerable<RoomRepository> GetRooms() => _listRooms;
    }
}
