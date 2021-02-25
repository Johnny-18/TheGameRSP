using System;
using System.Net.Http;
using RSPGame.Models;
using RSPGame.UI.PlayRequests;

namespace RSPGame.UI
{
    public class GameLogic
    {
        public void StartGame(HttpClient client, string[] usersName, int roomId)
        {
            var roundId = 0;

            roundId++;
            Console.Clear();
            Console.WriteLine($"Room ID:\t{roomId}");
            Console.WriteLine($"Round:\t{roundId}");
            Console.WriteLine($"Match:\t{string.Join(" vs ", usersName)}");
            Console.WriteLine("Rules:\tRock > scissors; scissors > paper; paper > rock.\n\n");

            int num;
            GameActionsUi action = GameActionsUi.None;

            Console.WriteLine("1.\tRock");
            Console.WriteLine("2.\tScissors");
            Console.WriteLine("3.\tPaper");
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
                    action = GameActionsUi.Rock;
                    break;
                case 2:
                    action = GameActionsUi.Scissors;
                    break;
                case 3:
                    action = GameActionsUi.Paper;
                    break;
                case 4:
                    return;
            }

            if (GameRequests.PostAction(client, action, roomId, roundId) == null) 
                ;

        }
    }
}
