using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Prototype
{
    public class RoomPrototype
    {
        private readonly int _id = new Random().Next(0, 1000);

        private readonly List<GameInfo> _gamers;

        public RoomPrototype()
        {
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
                    break;
                }
            }
        }

        private Task StartGame()
        {
            Console.WriteLine("Rock > scissors; scissors > paper; paper > rock.");
            //solution
            
            return Task.CompletedTask;
        }

        public Task AddGamer(GameInfo gamer)
        {
            if (gamer == null) throw new ArgumentNullException();

            _gamers.Add(gamer);
            return Task.CompletedTask;
        }

        public int GetId() => _id;
    }
}
