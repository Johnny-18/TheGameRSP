using RSPGame.Models;
using RSPGame.UI.PlayRequests;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace RSPGame.UI.Menus
{
    public class AuthorizationMenu
    {
        private Session _currentSession;

        private readonly HttpClient _client;

        private readonly Stopwatch _stopwatch;

        public AuthorizationMenu(HttpClient client, Session currentSession)
        {
            _client = client;
            _currentSession = currentSession;
            _stopwatch = new Stopwatch();
        }

        public async Task Start()
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
                    _currentSession.CountLoginFailed = 0;
                    _stopwatch.Stop();
                }
              
                switch (num)
                {
                    case 1:
                        await AuthRequests.Register(_client, _currentSession);
                        break;
                    case 2:
                        if (_currentSession.CountLoginFailed < 3)
                        {
                            await AuthRequests.Login(_client, _currentSession, _stopwatch);
                        }
                        else
                        {
                            Console.WriteLine("You were temporarily blocked due to incorrect authorization!");
                        }
                        break;
                    case 3:
                        await StatRequests.GetGeneralStat(_client);
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