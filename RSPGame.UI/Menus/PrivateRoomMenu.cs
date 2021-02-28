using System;
using System.Diagnostics;
using System.Net;
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
        private readonly Session _currentSession;

        private readonly HttpClient _client;

        private readonly Stopwatch _onlineTime = new Stopwatch();

        public PrivateRoomMenu(HttpClient client, Session currentSession)
        {
            _client = client;
            _currentSession = currentSession;
        }

        public void Start()
        {
            _onlineTime.Start();
            Console.Clear();
            
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
                        if (_currentSession?.GamerInfo == null)
                            break;
                        
                        var json = JsonConvert.SerializeObject(_currentSession.GamerInfo);
                        var requestOptions = new RequestOptions
                        {
                            Address = _client.BaseAddress + "api/rooms/create",
                            Body = json,
                            Method = RequestMethod.Post,
                            Token = _currentSession.Token
                        };

                        var response = RequestHandler.HandleRequest(_client, requestOptions);
                        if (response == null)
                            return;
                        
                        if (response.StatusCode == (int) HttpStatusCode.Unauthorized)
                        {
                            Console.WriteLine("You need to login! Or register your account!");
                            return;
                        }
                        
                        var content = response.Content;
                        if (string.IsNullOrEmpty(content))
                        {
                            break;
                        }

                        var roomId = JsonConvert.DeserializeObject<int>(content);

                        Console.WriteLine($"\nRoom with id {roomId} has been created!");
                        Console.WriteLine("\nWaiting for opponent\n\n");

                        while (true)
                        {
                            var gamerInfos = GameRequests.GetGamers(_client, _currentSession.Token, roomId, 30);
                            if (gamerInfos != null && gamerInfos.Length == 2)
                            {
                                new GameLogic().StartGame(_client, gamerInfos, _currentSession, roomId);
                                break;
                            }

                            RoomRequests.DeleteRoom(_client, _currentSession.Token, roomId);
                            Console.WriteLine("\nOpponent do not found!\n");
                            break;
                        }
                        
                        _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                        _onlineTime.Restart();
                        break;
                    case 2:
                        if(_currentSession?.GamerInfo == null)
                            break;
                        
                        Console.Write("Enter the id of the desired room: ");

                        if (!int.TryParse(Console.ReadLine(), out var id2))
                        {
                            Console.WriteLine("\nERROR:\tThe only numbers can be entered. Try again\n");
                            break;
                        }
                        else if (id2 < 0)
                        {
                            Console.WriteLine("\nERROR:\tIncorrect number. Try again\n");
                            break;
                        }
                        
                        if (RoomRequests.JoinRoom(_client, _currentSession, id2) == false) 
                            break;

                        var gamers = GameRequests.GetGamers(_client, _currentSession.Token, id2, 3);
                        
                        new GameLogic().StartGame(_client, gamers, _currentSession, id2);
                        
                        _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                        _onlineTime.Restart();
                        break;
                    case 3:
                        if (_currentSession?.GamerInfo == null)
                            break;
                            
                        _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                        _onlineTime.Restart();
                        
                        StatRequests.SaveOnlineTime(_client, _currentSession);
                        return;
                }
            }
        }
    }
}