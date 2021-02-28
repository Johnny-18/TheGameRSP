using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RSPGame.Models.GameModel;

namespace RSPGame.Storage
{
    public class RoundStorage
    {
        private readonly ConcurrentDictionary<int, BlockingCollection<GamerStep>> _dictionaryRound = new();

        public bool ContainRoom(int id)
        {
            if (_dictionaryRound.ContainsKey(id))
                return true;

            return false;
        }

        public void AddGamer(int id, GamerStep round)
        {
            _dictionaryRound.TryAdd(id, new BlockingCollection<GamerStep>(2));

            if (_dictionaryRound[id].Count == 2)
                _dictionaryRound[id] = new BlockingCollection<GamerStep>(2);

            _dictionaryRound[id].Add(round);
        }

        public IEnumerable<GamerStep> PeekGamers(int id)
        {
            if (!_dictionaryRound.ContainsKey(id))
                return null;

            return _dictionaryRound[id];
        }

        public void DeleteGamers(int id)
        {
            if (_dictionaryRound.ContainsKey(id))
            {
                var gamers = _dictionaryRound[id];
                _dictionaryRound.TryRemove(id, out gamers);
            }
        }
    }
}
