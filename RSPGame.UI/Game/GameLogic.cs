using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

            //var json = JsonConvert.SerializeObject(action);

            //var content = new StringContent(json, Encoding.UTF8, "application/json");

            //await client.PostAsync($"/api/round/{roomId}/{userName}", content);

            //var response = await client.GetAsync($"/api/round/{roomId}/{userName}");

            //json = await response.Content.ReadAsStringAsync();

            //var result = JsonConvert.DeserializeObject<GameActionsUi>(json);

            //Console.WriteLine($"Result:\t{result}");

            //result = message.Content.ReadAsStringAsync();

            //if (GameRequests.PostAction(client, action, roomId, roundId) == null) 
            //    return;

        }

        public Task PlayWithBotAsync(HttpClient client)
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
                    Console.WriteLine("Goodbye!\n");
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
