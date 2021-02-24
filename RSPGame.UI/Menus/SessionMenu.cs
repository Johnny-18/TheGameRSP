using System;
using System.Net.Http;
using RSPGame.Models;

namespace RSPGame.UI.Menus
{
    public static class SessionMenu
    {
        public static void Start(HttpClient client, GamerInfo gamer)
        {
            while (true)
            {
                int num;
                Console.WriteLine("1.\tPlay");
                Console.WriteLine("2.\tStatistics");
                Console.WriteLine("3.\tMy statistics");
                Console.WriteLine("4.\tLogout");
                Console.WriteLine("5.\tExit");

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
                        PlayMenu.Start(client, gamer);
                        break;
                    case 2:
                        //
                        break;
                    case 3:
                        //
                        break;
                    case 4:
                        return;
                    case 5:
                        //
                        break;
                }
            }
        }
    }
}
