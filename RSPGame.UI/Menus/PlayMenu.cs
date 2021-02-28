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
    public class PlayMenu
    {
        private readonly Session _currentSession;

        private readonly HttpClient _client;

        private readonly Stopwatch _onlineTime = new Stopwatch();

        public PlayMenu(HttpClient client, Session currentSession)
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
                        if (_currentSession?.GamerInfo == null)
                            break;
                        
                        var json = JsonConvert.SerializeObject(_currentSession.GamerInfo);
                        var requestOptions = new RequestOptions
                        {
                            Body = json,
                            Address = _client.BaseAddress +  "api/rooms/join",
                            Method = RequestMethod.Post,
                            Token = _currentSession.Token
                        };

                        var response = RequestHandler.HandleRequest(_client, requestOptions);
                        if(response == null)
                            break;
                        
                        if (response.StatusCode == (int) HttpStatusCode.Unauthorized)
                        {
                            Console.WriteLine("You need to login! Or register your account!");
                            return;
                        }
                        
                        var content = response.Content;
                        if (string.IsNullOrEmpty(content)) 
                            break;

                        var roomId = JsonConvert.DeserializeObject<int>(content);
                        Console.WriteLine($"You will play in room {roomId}!");
                        Console.WriteLine($"Waiting for opponent!");

                        var gamers = GameRequests.GetGamers(_client, _currentSession.Token, roomId, 30);
                        if (gamers == null || gamers.Length != 2)
                        {
                            Console.WriteLine("Game canceled because opponent did not found!");
                            break;
                        }
                        
                        new GameLogic().StartGame(_client, gamers, _currentSession, roomId);
                        
                        _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                        _onlineTime.Restart();
                        break;
                    case 2:
                        if (_currentSession?.GamerInfo == null)
                            break;
                        
                        new PrivateRoomMenu(_client, _currentSession).Start();
                        
                        _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                        _onlineTime.Restart();
                        break;
                    case 3:
                        if (_currentSession?.GamerInfo == null)
                            break;
                        
                        new GameLogic().PlayWithBot(_client, _currentSession.Token);
                        
                        _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                        _onlineTime.Restart();
                        break;
                    case 4:
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