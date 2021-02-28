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

        public AuthorizationMenu(HttpClient client, Session currentSession)
        {
            _client = client;
            _currentSession = currentSession;
            _stopwatch = new Stopwatch();
        }

        public void Start()
        {
            Console.Clear();

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
                        break;
                    case 3:
                        StatRequests.GetGeneralStat(_client);
                        break;
                    case 4:
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Incorrect number. Try again");
                        break;
                }
            }
        }
    }
}