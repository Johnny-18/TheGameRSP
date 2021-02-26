using System.Collections.Generic;
using RSPGame.Models.RoomModel;

namespace RSPGame.Storage
{
    public class RoomStorage
    {
        public RoomStorage()
        {
            Rooms = new List<RoomRepository>();
        }
        
        public List<RoomRepository> Rooms { get; set; }
    }
}
