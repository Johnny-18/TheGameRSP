using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RSPGame.Models
{
    public class Room
    {
        private readonly int _id = new Random().Next(0, 1000);

        private readonly List<GamerInfo> _gamers;

        public Room()
        {
            _gamers = new List<GamerInfo>();
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

        public Task AddGamer(GamerInfo gamer)
        {
            if (gamer == null) 
                throw new ArgumentNullException(nameof(gamer));

            _gamers.Add(gamer);
            return Task.CompletedTask;
        }

        public int GetId() => _id;
    }
}
