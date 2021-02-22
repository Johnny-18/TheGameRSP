using System;

namespace RSPGame.UI.Menus
{
    public static class PlayMenu
    {
        public static void Start()
        {
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
                    if (!int.TryParse(Console.ReadLine(), out num)) Console.WriteLine("The only numbers can be entered. Try again");
                    else if (num < 1 || num > 4) Console.WriteLine("Incorrect number. Try again");
                    else break;
                }
                Console.WriteLine();
                switch (num)
                {
                    case 1:
                        //
                        break;
                    case 2:
                        PrivateRoomMenu.Start();
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
