using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.UI.PlayRequests;

namespace RSPGame.UI.Game
{
    public class GameLogic
    {
        public async void StartGame(HttpClient client, string userName, string opponentName, int roomId)
        {
            var roundId = 0;
            var seriesSw = new Stopwatch();
            seriesSw.Start();

            //series
            while (true)
            {
                roundId++;

                PrintPreview(roomId, roundId, userName, opponentName);

                //var cancelable = new CancellationTokenSource();

                //var roundTask = Task.Run(() => StartRound(client, userName, roomId), cancelable.Token);

                StartRound(client, userName, roomId); //, cancelable.Token);

                // while (true)
                // {
                //     if (seriesSw.Elapsed.Minutes >= 5)
                //     {
                //         //todo close game logic
                //         return;
                //     }
                //     else
                //     {
                //         if (roundTask.IsCompleted)
                //         {
                //             var response = await GameRequests.RequestWithTimer(client, $"/api/round/{roomId}/{userName}", 1);
                //
                //             var json = await response.Content.ReadAsStringAsync();
                //
                //             var roundResult = JsonConvert.DeserializeObject<RoundResult>(json);
                //             if (roundResult == RoundResult.None)
                //             {
                //                 //cancel round
                //                 return;
                //             }
                //         }
                //     }
                // }
            }
        }

        private void StartRounds(HttpClient client, string userName, string opponentName, int roomId)
        {
            
        }

        private void PrintPreview(int roomId, int roundId, string userName, string opponentName)
        {
            Console.Clear();
            Console.WriteLine($"RoomRepository ID:{roomId}");
            Console.WriteLine($"Round:\t{roundId}");
            Console.WriteLine($"Match:\t{userName} vs {opponentName}");
            Console.WriteLine("Rules:\tRock > scissors; scissors > paper; paper > rock.\n\n");
        }

        private GameActionsUi GetAction()
        {
            int num;

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
                    return GameActionsUi.Rock;
                case 2:
                    return GameActionsUi.Scissors;
                case 3:
                    return GameActionsUi.Paper;
                default:
                    return GameActionsUi.None;
            }
        }

        private async void StartRound(HttpClient client, string userName, int roomId)
        {
            GameActionsUi action = GetAction();

            string json;
            try
            {
                json = JsonConvert.SerializeObject(action);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //send action
                await client.PostAsync($"/api/round/{roomId}/{userName}", content);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return;
            }
            ////////////////////////////////////////////

            //check on opponent step
            var response = await GameRequests.RequestWithTimer(client, $"/api/round/{roomId}/{userName}", 20);
            if (response == null)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("\nERROR:\tServer problems.\n\n");
                return;
            }

            json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<RoundResult>(json);
            if (result == RoundResult.None)
            {
                //todo cancel round
            }

            PrintResult(result);
        }

        public async Task PlayWithBotAsync(HttpClient client)
        {
            Console.Clear();

            GameActionsUi action = GetAction();

            var json = JsonConvert.SerializeObject(action);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/bot", content);

            if (response.StatusCode == HttpStatusCode.OK)
                json = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<RoundResult>(json);

            PrintResult(result);
        }

        private void PrintResult(RoundResult result)
        {
            switch (result)
            {
                case RoundResult.Win:
                    Console.WriteLine("\nCongratulation! You win!\n\n");
                    break;
                case RoundResult.Lose:
                    Console.WriteLine("\nUnfortunately, you lose!\n\n");
                    break;
                case RoundResult.Draw:
                    Console.WriteLine("\nDraw!\n\n");
                    break;
            }
        }
    }
}
