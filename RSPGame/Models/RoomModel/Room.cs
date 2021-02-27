using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace RSPGame.Models.RoomModel
{
    [Serializable]
    public class Room
    {
        private static int _id;

        static Room()
        {
            _id = 1;
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

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("gamers")]
        public BlockingCollection<GamerInfo> Gamers { get; set; }

        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; }

        private void DefaultInitialize()
        {
            Id = _id;
            _id++;
            
            Gamers = new BlockingCollection<GamerInfo>(2);
        }
    }
}
