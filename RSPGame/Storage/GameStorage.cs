using System.Collections.Concurrent;

namespace RSPGame.Storage
{
    public class GameStorage
    {
        public ConcurrentDictionary<int, string[]> DictionaryGame { get; set; } = new();
    }
}
