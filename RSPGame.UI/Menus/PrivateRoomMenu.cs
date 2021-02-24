using System;
using System.Net.Http;
using RSPGame.Models;

namespace RSPGame.UI.Menus
{
    public static class PrivateRoomMenu
    {
        public static void Start(HttpClient client, GamerInfo gamer)
        {
            while (true)
            {
                int num;
                Console.WriteLine("1.\tCreate room");
                Console.WriteLine("2.\tJoin room");
                Console.WriteLine("3.\tBack");

                while (true)
                {
                    Console.Write("Enter the number: ");
                    if (!int.TryParse(Console.ReadLine(), out num)) Console.WriteLine("The only numbers can be entered. Try again");
                    else if (num < 1 || num > 3) Console.WriteLine("Incorrect number. Try again");
                    else break;
                }
                Console.WriteLine();
                switch (num)
                {
                    case 1:
                        //
                        break;
                    case 2:
                        //
                        break;
                    case 3:
                        return;
                }
            }
        }
    }
}
