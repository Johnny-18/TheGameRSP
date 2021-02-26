using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.UI.PlayRequests;

namespace RSPGame.UI.Game
{
    public class GameLogic
    {
        public void StartGame(HttpClient client, string userName, string opponentName, int roomId)
        {
            var roundId = 0;
            roundId++;
            Console.Clear();
            Console.WriteLine($"Room ID:{roomId}");
            Console.WriteLine($"Round:\t{roundId}");
            Console.WriteLine($"Match:\t{userName} vs {opponentName}");
            Console.WriteLine("Rules:\tRock > scissors; scissors > paper; paper > rock.\n\n");

            StartRound(client, userName, roomId);
        }

        public void StartRound(HttpClient client, string userName, int roomId)
        {
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

            string json;
            HttpResponseMessage response;

            try
            {
                json = JsonConvert.SerializeObject(action);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = client.PostAsync($"/api/round/{roomId}/{userName}", content).Result;
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return;
            }

            response = GameRequests.RequestWithTimer(client, $"/api/round/{roomId}/{userName}", 20);
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

            json = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<RoundResult>(json);

            switch (result)
            {
                case RoundResult.None:
                    Console.WriteLine("\nResult:\tNone\n\n");
                    break;
                case RoundResult.Draw:
                    Console.WriteLine("\nResult:\tDraw\n\n");
                    break;
                case RoundResult.Win:
                    Console.WriteLine("\nResult:\tWin\n\n");
                    break;
                case RoundResult.Lose:
                    Console.WriteLine("\nResult:\tLose\n\n");
                    break;
            }

            //Console.WriteLine($"Result:\t{result}");

            //result = message.Content.ReadAsStringAsync();

            //if (GameRequests.PostAction(client, action, roomId, roundId) == null) 
            //    return;

        }

        public Task PlayWithBotAsync(HttpClient client)
        {
            Console.Clear();

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
                    return Task.CompletedTask;
            }

            var json = JsonConvert.SerializeObject(action);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = client.PostAsync($"/api/game/bot", content).Result;

            if (response.StatusCode == HttpStatusCode.OK)
                json = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<RoundResult>(json);

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
            return Task.CompletedTask;
        }
    }
}
