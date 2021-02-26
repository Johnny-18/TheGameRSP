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
                        var json = await RoomRequests.QuickSearch(_client, _currentSession.GamerInfo);
                        if (json == null) 
                            break;
                        
                        var roomId = JsonConvert.DeserializeObject<int>(json);
                        Console.WriteLine($"You will play in room {roomId}!");

                        var gamers = await GameRequests.GetGame(_client, roomId);
                        
                        await new GameLogic().StartGame(_client, gamers, roomId);
                        break;
                    case 2:
                        await new PrivateRoomMenu(_client, _currentSession).Start();
                        break;
                    case 3:
                        //
                        break;
                    case 4:
                        return;
                }
            }
        }
    }
}