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

        public async void Start()
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
                        var json = JsonConvert.SerializeObject(_currentSession.GamerInfo);
                        var requestOptions = new RequestOptions
                        {
                            Body = json,
                            Address = _client.BaseAddress +  "api/rooms/join",
                            Method = RequestMethod.Post
                        };

                        var response = RequestHandler.HandleRequest(_client, requestOptions);
                        var content = response.Content;
                        if (string.IsNullOrEmpty(content)) 
                            break;

                        var roomId = JsonConvert.DeserializeObject<int>(content);
                        Console.WriteLine($"You will play in room {roomId}!");
                        Console.WriteLine($"Waiting for opponent!");

                        var gamers = GameRequests.GetGamers(_client, roomId, 30);
                        if (gamers == null || gamers.Length != 2)
                        {
                            Console.WriteLine("Game canceled because opponent did not found!");
                            break;
                        }

                        while (true)
                        {
                            var seriesStopWatch = new Stopwatch();
                            seriesStopWatch.Start();

                            while (true)
                            {
                                if (seriesStopWatch.Elapsed.Minutes > 5)
                                {
                                    //todo save stat and delete room
                                    RoomRequests.SaveStatRounds(_client, roomId);
                                    RoomRequests.DeleteRoom(_client, roomId);
                                    break;
                                }

                                var round = await new GameLogic().StartGame(_client, gamers, _currentSession.UserName, roomId);
                                if (round != null)
                                {
                                    RoundRequests.AddRoundToRoom(_client, round, roomId);
                                    RoundRequests.RefreshRound(_client, roomId);
                                }

                                break;
                            }
                            
                            if (ContinueGame(_client, roomId))
                            {
                                //todo save stat and delete room
                                RoomRequests.SaveStatRounds(_client, roomId);
                                RoomRequests.DeleteRoom(_client, roomId);
                                break;
                            }
                        }

                        _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                        _onlineTime.Restart();
                        break;
                    case 2:
                        new PrivateRoomMenu(_client, _currentSession).Start();
                        _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                        _onlineTime.Restart();
                        break;
                    case 3:
                        new GameLogic().PlayWithBot(_client);
                        _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                        _onlineTime.Restart();
                        break;
                    case 4:
                        _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                        _onlineTime.Restart();
                        
                        StatRequests.SaveOnlineTime(_client, _currentSession);
                        return;
                }
            }
        }
        
        private bool ContinueGame(HttpClient client, int roomId)
        {
            var gamers = RoomRequests.GetGamers(client, roomId);
            if (gamers == null)
                return false;
            
            return gamers.Length == 2;
        }
    }
}