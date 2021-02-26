using System.Collections.Concurrent;
using System.Collections.Generic;
using RSPGame.Models.GameModel;

namespace RSPGame.Storage
{
    public class RoundStorage
    {
        private readonly ConcurrentDictionary<int, ConcurrentStack<GamerStep>> _dictionaryRound = new();

        public bool ContainRoom(int id)
        {
            if (_dictionaryRound.ContainsKey(id))
                return true;

            return false;
        }

        public void AddGamer(int id, GamerStep round)
        {
            bool adding = _dictionaryRound.TryAdd(id, new ConcurrentStack<GamerStep>());

            _dictionaryRound[id].Push(round);
        }

        public IEnumerable<GamerStep> PeekGamers(int id)
        {
            if (!_dictionaryRound.ContainsKey(id))
                return null;

            return _dictionaryRound[id];
        }
    }
}
