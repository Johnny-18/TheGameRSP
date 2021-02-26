using System.Collections.Concurrent;

namespace RSPGame.Models.RoomModel
{
    public class Room
    {
        private static int _id;

        static Room()
        {
            _id = 0;
        }

        public Room()
        {
            DefaultInitialize();
        }

        public Room(bool isPrivate)
        {
            DefaultInitialize();
            
            IsPrivate = isPrivate;
        }

        public int Id { get; set; }

        public BlockingCollection<GamerInfo> Gamers { get; set; }

        public bool IsPrivate { get; }

        private void DefaultInitialize()
        {
            Id = _id;
            _id++;
            
            Gamers = new BlockingCollection<GamerInfo>(2);
        }
    }
}
