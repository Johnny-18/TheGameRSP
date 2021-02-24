using System;
using System.Net.Http;
using RSPGame.Models;

namespace RSPGame.UI.Menus
{
    public static class AuthorizationMenu
    {
        private static readonly HttpClient _client;
        private static GamerInfo _gamer;

        static AuthorizationMenu()
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:5000")
            };
        }

        public static void Start()
        {
            while (true)
            {
                int num;
                Console.WriteLine("1.\tRegistration");
                Console.WriteLine("2.\tLogin");
                Console.WriteLine("3.\tStatistics");
                Console.WriteLine("4.\tExit");

                while (true)
                {
                    Console.Write("Enter the number: ");
                    if (!int.TryParse(Console.ReadLine(), out num)) Console.WriteLine("The only numbers can be entered. Try again");
                    else if (num < 1 || num > 4) Console.WriteLine("Incorrect number. Try again");
                    else break;
                }
                Console.WriteLine();
                switch (num)
                {
                    case 1:
                        SessionMenu.Start(_client, _gamer);
                        break;
                    case 2:
                        SessionMenu.Start(_client, _gamer);
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
