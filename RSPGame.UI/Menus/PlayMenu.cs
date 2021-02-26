using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.UI.Game;
using RSPGame.UI.Models;
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

        public void Start()
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
                    if (!int.TryParse(Console.ReadLine(), out num)) 
                        Console.WriteLine("The only numbers can be entered. Try again");
                    else if (num < 1 || num > 4) 
                        Console.WriteLine("Incorrect number. Try again");
                    else 
                        break;
                }
                
                Console.WriteLine();
                switch (num)
                {
                    case 1:
                        var json = JsonConvert.SerializeObject(_currentSession.GamerInfo);
                        var requestOptions = new RequestOptions
                        {
                            Body = json,
                            Address = "api/rooms/join",
                            Method = RequestMethod.Post
                        };
                        
                        var content = RoomRequests.Post(_client, requestOptions);
                        if (string.IsNullOrEmpty(content)) 
                            break;
                        
                        var roomId = JsonConvert.DeserializeObject<int>(content);
                        Console.WriteLine($"You will play in room {roomId}!");

                        var gamers = GameRequests.GetGame(_client, roomId);
                        if (gamers == null || gamers.Length != 2)
                        {
                            break;
                        }
                        
                        new GameLogic().StartGame(_client, gamers, roomId);
                        break;
                    case 2:
                        new PrivateRoomMenu(_client, _currentSession).Start();
                        break;
                    case 3:
                        new GameLogic().PlayWithBotAsync(_client);
                        break;
                    case 4:
                        return;
                }
            }
        }
    }
}