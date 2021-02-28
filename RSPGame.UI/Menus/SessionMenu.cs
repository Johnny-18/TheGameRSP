using System;
using System.Diagnostics;
using System.Net.Http;
using RSPGame.Models;
using RSPGame.UI.PlayRequests;

namespace RSPGame.UI.Menus
{
    public class SessionMenu
    {
        private readonly Session _currentSession;

        private readonly HttpClient _client;

        private readonly Stopwatch _onlineTime = new Stopwatch();

        public SessionMenu(HttpClient client, Session currentSession)
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
                Console.WriteLine("1.\tPlay");
                Console.WriteLine("2.\tStatistics");
                Console.WriteLine("3.\tMy statistics");
                Console.WriteLine("4.\tLogout");

                while (true)
                {
                    Console.Write("Enter the number: ");
                    if (!int.TryParse(Console.ReadLine(), out num)) Console.WriteLine("The only numbers can be entered. Try again");
                    else if (num < 1 || num > 5) Console.WriteLine("Incorrect number. Try again");
                    else break;
                }
                Console.WriteLine();
                switch (num)
                {
                    case 1:
                        new PlayMenu(_client, _currentSession).Start();
                        
                        _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                        _onlineTime.Restart();
                        break;
                    case 2:
                        StatRequests.GetGeneralStat(_client);
                        
                        _currentSession.GamerInfo.OnlineTime += _onlineTime.Elapsed;
                        _onlineTime.Restart();
                        break;
                    case 3:
                        Console.WriteLine(_currentSession.GamerInfo.ToString());
                        
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
    }
}