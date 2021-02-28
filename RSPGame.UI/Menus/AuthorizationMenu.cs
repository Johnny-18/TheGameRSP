using System;
using System.Diagnostics;
using System.Net.Http;
using RSPGame.Models;
using RSPGame.UI.PlayRequests;

namespace RSPGame.UI.Menus
{
    public class AuthorizationMenu
    {
        private readonly Session _currentSession;

        private readonly HttpClient _client;

        private readonly Stopwatch _stopwatch;

        private int _countLoginFailed;

        private readonly Stopwatch _onlineTime = new Stopwatch();

        public AuthorizationMenu(HttpClient client, Session currentSession)
        {
            _client = client;
            _currentSession = currentSession;
            _stopwatch = new Stopwatch();
        }

        public void Start()
        {
            Console.Clear();
            _onlineTime.Start();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Start menu");
                Console.WriteLine("1.\tRegistration");
                Console.WriteLine("2.\tLogin");
                Console.WriteLine("3.\tStatistics");
                Console.WriteLine("4.\tExit");

                Console.Write("Enter the number: ");
                if (!int.TryParse(Console.ReadLine(), out var num))
                {
                    Console.WriteLine("The only numbers can be entered. Try again");
                    continue;
                }

                if (_stopwatch.ElapsedMilliseconds > 30000)
                {
                    _countLoginFailed = 0;
                    _stopwatch.Stop();
                }
              
                switch (num)
                {
                    case 1:
                        AuthRequests.Register(_client, _currentSession);
                        
                        if(_currentSession?.GamerInfo != null)
                        {
                            _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                            _onlineTime.Restart();
                        }
                        break;
                    case 2:
                        if (_countLoginFailed < 3)
                        {
                            AuthRequests.Login(_client, _currentSession, _stopwatch, ref _countLoginFailed);
                        }
                        else
                        {
                            Console.WriteLine("You were temporarily blocked due to incorrect authorization!");
                        }
                        
                        if(_currentSession?.GamerInfo != null)
                        {
                            _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                            _onlineTime.Restart();
                        }
                        break;
                    case 3:
                        StatRequests.GetGeneralStat(_client);
                        
                        if(_currentSession?.GamerInfo != null)
                        {
                            _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                            _onlineTime.Restart();
                        }
                        break;
                    case 4:
                        Console.WriteLine("Goodbye!");
                        
                        if(_currentSession?.GamerInfo != null)
                        {
                            _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                            _onlineTime.Restart();
                        }
                        return;
                    default:
                        Console.WriteLine("Incorrect number. Try again");
                        
                        if(_currentSession?.GamerInfo != null)
                        {
                            _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                            _onlineTime.Restart();
                            StatRequests.SaveOnlineTime(_client, _currentSession);
                        }
                        break;
                }
            }
        }
    }
}