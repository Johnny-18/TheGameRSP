using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.UI.Game;
using RSPGame.UI.PlayRequests;

namespace RSPGame.UI.Menus
{
    public class PlayMenu
    {
        private Session _currentSession;

        private readonly HttpClient _client;

        public PlayMenu(HttpClient client, Session currentSession)
        {
            _client = client;
            _currentSession = currentSession;
        }

        public Task Start()
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
                        var json = RoomRequests.QuickSearch(_client, _currentSession.GamerInfo)?.Result;
                        if (json == null) break;
                        var id = JsonConvert.DeserializeObject<int>(json);

                        var result = GameRequests.GetGame(_client, id)?.ToArray();
                        if (result == null) break;

                        var opponent1 = result
                            .Where(x => !x.Equals(_currentSession.GamerInfo.UserName))?
                            .First();

                        new GameLogic().StartGame(_client, _currentSession.GamerInfo.UserName, opponent1, id);
                        break;
                    case 2:
                        new PrivateRoomMenu(_client, _currentSession).Start();
                        break;
                    case 3:
                        new GameLogic().PlayWithBotAsync(_client);
                        break;
                    case 4:
                        return Task.CompletedTask;
                }
            }
        }
    }
}