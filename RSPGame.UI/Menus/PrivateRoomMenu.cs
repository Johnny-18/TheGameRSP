using System;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.UI.Game;
using RSPGame.UI.Models;
using RSPGame.UI.PlayRequests;

namespace RSPGame.UI.Menus
{
    public class PrivateRoomMenu
    {
        private Session _currentSession;

        private readonly HttpClient _client;

        public PrivateRoomMenu(HttpClient client, Session currentSession)
        {
            _client = client;
            _currentSession = currentSession;
        }

        public void Start()
        {
            while (true)
            {
                int num;
                Console.WriteLine("1.\tCreate room");
                Console.WriteLine("2.\tJoin room");
                Console.WriteLine("3.\tBack");

                while (true)
                {
                    Console.Write("Enter the number: ");
                    if (!int.TryParse(Console.ReadLine(), out num)) 
                        Console.WriteLine("The only numbers can be entered. Try again");
                    else if (num < 1 || num > 3) 
                        Console.WriteLine("Incorrect number. Try again");
                    else 
                        break;
                }
                Console.WriteLine();
                switch (num)
                {
                    case 1:
                        //
                        var json = JsonConvert.SerializeObject(_currentSession.GamerInfo);
                        var requestOptions = new RequestOptions
                        {
                            Address = _client.BaseAddress + "api/rooms/create",
                            Body = json,
                            Method = RequestMethod.Post
                        };

                        var content = RoomRequests.Post(_client, requestOptions);

                        var roomId = JsonConvert.DeserializeObject<int>(content);

                        Console.WriteLine($"\nRoom with id {roomId} has been created!");
                        Console.WriteLine("\nWaiting for opponent\n\n");
                        
                        var period = new Stopwatch();
                        
                        period.Start();
                        
                        while (true)
                        {
                            var gamerInfos = GameRequests.GetGame(_client, roomId, 10);
                            if (gamerInfos != null && gamerInfos.Length == 2)
                            {
                                new GameLogic().StartGame(_client, gamerInfos, _currentSession.UserName, roomId);
                                break;
                            }

                            RoomRequests.DeleteRoom(_client, roomId);
                            Console.WriteLine("Opponent do not found!");
                            break;
                        }
                        break;
                    case 2:
                        Console.Write("Enter the id of the desired room: ");

                        if (!int.TryParse(Console.ReadLine(), out var id2))
                        {
                            Console.WriteLine("\nERROR:\tThe only numbers can be entered. Try again\n\n");
                            break;
                        }
                        else if (id2 < 0)
                        {
                            Console.WriteLine("\nERROR:\tIncorrect number. Try again\n\n");
                            break;
                        }
                        
                        if (RoomRequests.JoinRoom(_client, _currentSession.GamerInfo, id2)) 
                            break;

                        var gamers = GameRequests.GetGame(_client, id2, 30);
                        
                        new GameLogic().StartGame(_client, gamers, _currentSession.UserName, id2);
                        break;
                    case 3:
                        return;
                }
            }
        }
    }
}