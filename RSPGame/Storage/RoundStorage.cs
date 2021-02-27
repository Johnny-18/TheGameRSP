using System.Collections.Concurrent;
using System.Collections.Generic;
using RSPGame.Models.GameModel;

namespace RSPGame.Storage
{
    public class RoundStorage
    {
        public RoundStorage()
        {
            _dictionaryRound = new ConcurrentDictionary<int, BlockingCollection<GamerStep>>();
        }

        /// <summary>
        /// Dictionary where int is room id, BlockingCollection<GamerStep> are user actions.
        /// </summary>
        private readonly ConcurrentDictionary<int, BlockingCollection<GamerStep>> _dictionaryRound;

        public bool ContainRoom(int id)
        {
            if (_dictionaryRound.ContainsKey(id))
                return true;

            return false;
        }

        public void AddGamer(int id, GamerStep round)
        {
            bool adding = _dictionaryRound.TryAdd(id, new BlockingCollection<GamerStep>(2));

            _dictionaryRound[id].Add(round);
        }

        public IEnumerable<GamerStep> PeekGamers(int id)
        {
            if (!_dictionaryRound.ContainsKey(id))
                return null;

            return _dictionaryRound[id];
        }
    }
}
