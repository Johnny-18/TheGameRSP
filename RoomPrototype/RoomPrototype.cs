using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RoomPrototype
{
    public class RoomPrototype : IDisposable
    {
        private readonly int _id = 0;

        private readonly List<GameInfo> _gamers;

        public RoomPrototype()
        {
            _gamers = new List<GameInfo>();
            Task.Run(GamersCheck);
        }

        public RoomPrototype(int id)
        {
            _id = id;
            _gamers = new List<GameInfo>();
            Task.Run(GamersCheck);
        }

        private async void GamersCheck()
        {
            while (true)
            {
                if (_gamers.Count == 2)
                {
                    await StartGame();
                    Dispose();
                    break;
                }
            }
        }

        private Task StartGame()
        {
            //solution
            return Task.CompletedTask;
        }

        public Task AddGamer(GameInfo gamer)
        {
            if (gamer == null) throw new ArgumentNullException();

            _gamers.Add(gamer);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
