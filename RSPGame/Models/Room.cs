using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RSPGame.Models
{
    public class Room
    {
        private readonly int _id = new Random().Next(1, 1001);

        private readonly RoomStatus _roomStatus;

        private readonly List<GamerInfo> _gamers;

        private readonly object _locker = new();

        public Room(RoomStatus roomStatus)
        {
            _roomStatus = roomStatus;
            _gamers = new List<GamerInfo>();
            Task.Run(GamersCheck);
            //var timer = new Timer(GamersCheck, null, 0, 100);
        }

        private async void GamersCheck()
        {
            while (true)
            {
                if (_gamers.Count != 2) continue;
                await StartGame();
                break;
            }

        }

        private async Task StartGame()
        {
            using var client = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:5000")
            };

            var json = JsonSerializer.Serialize(
                _gamers.Select(x => x.UserName).ToArray()
                );

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PostAsync($"/api/game/{_id}", content);

        }

        public Task AddGamer(GamerInfo gamer)
        {
            lock (_locker)
            {
                if (gamer == null)
                    throw new ArgumentNullException(nameof(gamer));

                _gamers.Add(gamer);
                return Task.CompletedTask;
            }
        }

        public int GetId() => _id;

        public bool IsPublic() => _roomStatus == RoomStatus.Public;
    }
}
