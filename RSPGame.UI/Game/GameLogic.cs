using Newtonsoft.Json;
using RSPGame.UI.PlayRequests;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RSPGame.Models.GameModel;

namespace RSPGame.UI.Game
{
    public class GameLogic
    {
        private const int StdInputHandle = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);

        public void StartGame(HttpClient client, string userName, string opponentName, int roomId)
        {
            var roundId = 0;

            while (true)
            {
                GamerInfo gamer = new GamerInfo()
                {
                    UserName = userName
                };
                roundId++;

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                PrintPreview(roomId, roundId, userName, opponentName);
                PostGamerAction(client, userName, roomId, gamer);
                var roundResult = GetRoundResult(client, userName, roomId, gamer);

                gamer.OnlineTime += stopwatch.Elapsed;

                if (roundResult != RoundResult.None)
                    SaveGamerInfo(client, userName, gamer);

                int num;
                Console.WriteLine("1.\tContinue");
                Console.WriteLine("2.\tExit");
                Console.WriteLine("Do you want to continue?");
                while (true)
                {
                    Console.Write("Enter the number: ");
                    if (!int.TryParse(Console.ReadLine(), out num)) Console.WriteLine("The only numbers can be entered. Try again");
                    else if (num < 1 || num > 3) Console.WriteLine("Incorrect number. Try again");
                    else break;
                }
                Console.WriteLine();

                if (num == 2)
                {
                    var r = client.DeleteAsync($"api/round/{roomId}");
                    break;
                }

                var content = new StringContent(JsonConvert.SerializeObject(userName), Encoding.UTF8, "application/json");
                client.PostAsync($"api/round/{roomId}", content);

                var response = GameRequests.RequestWithTimer(client, $"api/round/{roomId}", 10);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine("Unfortunately the second player don`t accept invite!\n\n");
                    client.DeleteAsync($"api/round/{roomId}");
                    break;
                }

                var json = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<bool>(json);

                if (!result)
                {
                    Console.WriteLine("Unfortunately the second player don`t accept invite!\n\n");
                    client.DeleteAsync($"api/round/{roomId}");
                    break;
                }
            }
        }

        private void SaveGamerInfo(HttpClient client, string userName, GamerInfo gamer)
        {
            var json = JsonConvert.SerializeObject(gamer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/stat/save", content).Result;
        }

        private void PostGamerAction(HttpClient client, string userName, int roomId, GamerInfo gamer)
        {
            GameActionsUi action = GameActionsUi.None;
            var read = false;
            Task.Delay(20000).ContinueWith(_ =>
            {
                if (!read)
                {
                    var handle = GetStdHandle(StdInputHandle);
                    CancelIoEx(handle, IntPtr.Zero);
                }
            });

            try
            {
                action = GetAction();
                read = true;
            }
            catch (InvalidOperationException)
            {
            }
            catch (OperationCanceledException)
            {
            }

            if (!read)
            {
                Console.WriteLine("\n\nTime is over! You haven't selected anything!");
            }

            switch (action)
            {
                case GameActionsUi.Rock:
                    gamer.CountRocks++;
                    break;
                case GameActionsUi.Paper:
                    gamer.CountPapers++;
                    break;
                case GameActionsUi.Scissors:
                    gamer.CountScissors++;
                    break;
            }

            try
            {
                var json = JsonConvert.SerializeObject(action);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync($"/api/round/{roomId}/{userName}", content).Result;
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
            }
        }

        private RoundResult GetRoundResult(HttpClient client, string userName, int roomId, GamerInfo gamer)
        {

            var response = GameRequests.RequestWithTimer(client, $"/api/round/{roomId}/{userName}", 20);
            if (response == null)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return RoundResult.None;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("\nERROR:\tServer problems.\n\n");
                return RoundResult.None;
            }

            var json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<RoundResult>(json);

            switch (result)
            {
                case RoundResult.Draw:
                    gamer.CountDraws++;
                    break;
                case RoundResult.Win:
                    gamer.CountWins++;
                    break;
                case RoundResult.Lose:
                    gamer.CountLoses++;
                    break;
            }

            PrintResult(result);
            return result;
        }

        private void PrintPreview(int roomId, int roundId, string userName, string opponentName)
        {
            Console.Clear();
            Console.WriteLine($"Room ID:{roomId}");
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
                default:
                    Console.WriteLine("\nRound was canceled!\n\n");
                    break;
            }
        }

        public async Task PlayWithBotAsync(HttpClient client)
        {
            Console.Clear();
            GameActionsUi action = GetAction();

            var json = JsonConvert.SerializeObject(action);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync($"/api/bot", content);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return;
            }

            if (response.StatusCode == HttpStatusCode.OK)
                json = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<RoundResult>(json);

            PrintResult(result);
        }

    }
}