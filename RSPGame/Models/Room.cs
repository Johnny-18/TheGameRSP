using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RSPGame.Models
{
    public class Room
    {
        private readonly int _id = new Random().Next(1, 1001);

        private readonly RoomStatus _roomStatus;

        private readonly List<GamerInfo> _gamers;

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

        private Task StartGame()
        {
            //HttpClient client = new HttpClient();

            //foreach (var gamer in _gamers)
            //{
            //    HttpResponseMessage response = await client.PostAsync(
            //        $"api/game/{gamer.UserId}", GetId());
            //    response.EnsureSuccessStatusCode();
            //}
            
            //solution

            //мы делаем запросы со всей интересующей нас
            //инфой /api/game/{id_игрока}
            //{ номер комнаты и противника }

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

        public bool IsPublic() => _roomStatus == RoomStatus.Public;
    }
}
