using System.Collections.Concurrent;
using RSPGame.Models;

namespace RSPGame.Storage
{
    public class RspRepository
    {
        public ConcurrentDictionary<string, User> Users { get; set; }
    }
}