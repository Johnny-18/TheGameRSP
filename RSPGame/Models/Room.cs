using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
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
            {
                await PostGameResponse(client, _gamers[0], _gamers[1]);
                await PostGameResponse(client, _gamers[1], _gamers[0]);
            }

            //solution

            //мы делаем запросы со всей интересующей нас
            //инфой /api/game/{id_игрока}
            //{ номер комнаты и противника }

        }

        private async Task PostGameResponse(HttpClient client, GamerInfo gamer1, GamerInfo gamer2)
        {
            StringContent content = new StringContent(
                JsonSerializer
                    .Serialize(new GameResponse(gamer2.UserName, _id))
            );

            await client.PostAsync(
                $"api/game/{gamer1.UserName}",
                content);
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
