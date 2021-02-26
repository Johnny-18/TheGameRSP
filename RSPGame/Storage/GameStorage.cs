using System.Collections.Concurrent;
using RSPGame.Models;

namespace RSPGame.Storage
{
    public class GameStorage
    {
        public ConcurrentDictionary<int, GamerInfo[]> DictionaryGame { get; set; } = new();
    }
}
