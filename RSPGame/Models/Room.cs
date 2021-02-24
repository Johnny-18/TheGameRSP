using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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

        private async Task StartGame()
        {
            using var client = new HttpClient();

            using var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:5000/api/game/{_id}"));

            var json = JsonSerializer.Serialize(
                new Game(new[] { _gamers[0].UserName, _gamers[1].UserName }, _id)
                );

            message.Content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.SendAsync(message);

            //var response = await client.GetAsync($"http://localhost:5000/api/game/{_id}");

            //solution

            //мы делаем запросы со всей интересующей нас
            //инфой /api/game/{id_игрока}
            //{ номер комнаты и противника }

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
