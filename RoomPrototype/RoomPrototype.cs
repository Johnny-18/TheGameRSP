using System;
using System.Collections.Generic;
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
                    break;
                }
            }
        }

        private Task StartGame()
        {
            //Console.WriteLine("game started!");

            //solution
            return Task.CompletedTask;
        }

        public Task AddGamer(GameInfo gamer)
        {
            if (gamer == null) throw new ArgumentNullException();

            //Console.WriteLine($"gamer {gamer.Id} added!");

            _gamers.Add(gamer);
            return Task.CompletedTask;
        }

    }
}
