using System.Collections.Concurrent;
using RSPGame.Models;

namespace RSPGame.Storage
{
    public class RoundStorage
    {
        public ConcurrentDictionary<int, BlockingCollection<UserAction>> DictionaryRound { get; set; } = new();
    }
}
