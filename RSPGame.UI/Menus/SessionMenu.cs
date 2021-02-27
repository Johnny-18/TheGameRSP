using System;
using System.Net.Http;
using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.Services.Statistics;
using RSPGame.UI.PlayRequests;

namespace RSPGame.UI.Menus
{
    public class SessionMenu
    {
        private Session _currentSession;

        private readonly HttpClient _client;

        public SessionMenu(HttpClient client, Session currentSession)
        {
            _client = client;
            _currentSession = currentSession;
        }

        public Task Start()
        {
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
                        break;
                    case 2:
                        StatRequests.GetGeneralStat(_client);
                        break;
                    case 3:
                        Console.WriteLine(_currentSession.GamerInfo.ToString());
                        break;
                    case 4:
                        return Task.CompletedTask;
                }
            }
        }
    }
}