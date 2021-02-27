using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.UI.Game;
using RSPGame.UI.PlayRequests;

namespace RSPGame.UI.Menus
{
    public class PlayMenu
    {
        private readonly Session _currentSession;

        private readonly HttpClient _client;

        public PlayMenu(HttpClient client, Session currentSession)
        {
            _client = client;
            _currentSession = currentSession;
        }

        public async Task Start()
        {
            while (true)
            {
                int num;
                Console.WriteLine("1.\tQuick search");
                Console.WriteLine("2.\tPrivate room");
                Console.WriteLine("3.\tWith bot");
                Console.WriteLine("4.\tBack");

                while (true)
                {
                    Console.Write("Enter the number: ");
                    if (!int.TryParse(Console.ReadLine(), out num)) Console.WriteLine("The only numbers can be entered. Try again");
                    else if (num < 1 || num > 4) Console.WriteLine("Incorrect number. Try again");
                    else break;
                }
                Console.WriteLine();
                switch (num)
                {
                    case 1:
                        QuickSearch();
                        break;
                    case 2:
                        await new PrivateRoomMenu(_client, _currentSession).Start();
                        break;
                    case 3:
                        await new GameLogic().PlayWithBotAsync(_client);
                        break;
                    case 4:
                        return;
                }
            }
        }

        public async void QuickSearch()
        {
            var json = await RoomRequests.PostAsync(_client, _currentSession.GamerInfo, "join");
            if (string.IsNullOrEmpty(json))
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return;
            }

            var id = JsonConvert.DeserializeObject<int>(json);

            var result = (await GameRequests.GetGame(_client, id))?.ToArray();
            if (result == null) return;

            var opponent = result
                .FirstOrDefault(x => !x.Equals(_currentSession.GamerInfo.UserName));

            new GameLogic().StartGame(_client, _currentSession.GamerInfo.UserName, opponent, id);

            await _client.DeleteAsync($"api/rooms/stop/{id}");
        }
    }
}