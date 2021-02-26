using System;
using System.Net.Http;
using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.UI.PlayRequests;
using System.Runtime.InteropServices;
using RSPGame.Models.Game;

namespace RSPGame.UI.Game
{
    public class GameLogic
    {
        private enum StdHandle { Stdin = -10, Stdout = -11, Stderr = -12 };
        
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(StdHandle std);
        
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hdl);
        
        public async Task StartGame(HttpClient client, GamerInfo[] gamers, int roomId)
        {
            // var firstGamer = gamers[0];
            // var secondGamer = gamers[1];

            Round round = null;
            
            await StartRound(client, gamers, roomId);

            Task waitingTask = Task.Delay(20000).ContinueWith(async _ =>
            {
                IntPtr stdin = GetStdHandle(StdHandle.Stdin);
                CloseHandle(stdin);
                Console.WriteLine("----------------------------------------------");
                //task.Dispose();
                
                round = await RoomRequests.GetLastRound(client, roomId);
                if (round != null)
                {
                    switch (@round.RoundResultForGamer1)
                    {
                        case RoundResult.Draw:
                            Console.WriteLine("Draw!");
                            break;
                        case RoundResult.Win:
                            Console.WriteLine("You Win!");
                            break;
                        case RoundResult.Lose:
                            Console.WriteLine("You Lose(");
                            break;
                    }
                }
            });

            await Task.WhenAll(waitingTask);

            var roomRep = await RoomRequests.GetRoomById(client, roomId);
            
            roomRep.SeriesRepository.AddRound(round);
        }

        private int GetNumberFromUser(string message)
        {
            while (true)
            {
                Console.WriteLine(message);
                if (int.TryParse(Console.ReadLine(), out var number) || number > 0 && number < 5)
                {
                    return number;
                }

                Console.WriteLine("Incorrect number. Try again");
            }
        }

        private async Task StartRound(HttpClient client, GamerInfo[] gamers, int roomId)
        {
            var firstGamer = gamers[0];
            var secondGamer = gamers[1];
            
            Console.Clear();
            Console.WriteLine($"Room ID:\t{roomId}");
            Console.WriteLine($"Match:\t{string.Join(firstGamer.UserName," vs ", secondGamer.UserName)}");
            Console.WriteLine("Rules:\tRock > scissors; scissors > paper; paper > rock.\n\n");

            int num;
            var action = GameActionsUi.None;

            Console.WriteLine("1.\tRock");
            Console.WriteLine("2.\tScissors");
            Console.WriteLine("3.\tPaper");
            Console.WriteLine("4.\tExit");


            var number = GetNumberFromUser("Enter the number: ");

            Console.WriteLine();
            switch (number)
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

            await RoomRequests.GameAction(client, firstGamer, action, roomId);
        }
    }
}
