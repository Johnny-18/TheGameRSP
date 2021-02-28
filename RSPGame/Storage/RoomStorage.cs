using System.Collections.Generic;
using RSPGame.Models.RoomModel;

namespace RSPGame.Storage
{
    public class RoomStorage
    {
        private readonly List<RoomRepository> _listRooms = new();
        private static readonly object Locker = new();

        public void AddRoom(RoomRepository roomRepository)
        {
            lock (Locker)
            {
                _listRooms.Add(roomRepository);
            }
        }

        public void RemoveRoom(RoomRepository roomRepository)
        {
            lock (Locker)
            {
                _listRooms.Remove(roomRepository);
            }
        }

        public IEnumerable<RoomRepository> GetRooms() 
        {
            lock (Locker)
            {
                return _listRooms;;
            }
        }
    }
}
