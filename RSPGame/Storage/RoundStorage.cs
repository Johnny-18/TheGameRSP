using System.Collections.Concurrent;
using RSPGame.Models;

namespace RSPGame.Storage
{
    public class RoundStorage
    {
        public ConcurrentDictionary<int, GameActions> DictionaryRound { get; set; } = new();
    }
}
