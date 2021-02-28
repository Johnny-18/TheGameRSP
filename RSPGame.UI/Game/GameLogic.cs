using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.UI.PlayRequests;
using System.Threading;
using Newtonsoft.Json;
using RSPGame.Models.Game;
using RSPGame.UI.Models;

namespace RSPGame.UI.Game
{
    public class GameLogic
    {
        public void PlayWithBot(HttpClient client)
        {
            var action = GetAction();

            var json = JsonConvert.SerializeObject(action);
            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + "api/game/bot",
                Method = RequestMethod.Post,
                Body = json
            };
            
            var response = RequestHandler.HandleRequest(client, requestOptions);
            if (response.StatusCode == (int) HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("You need to login! Or register your account!");
                return;
            }
            
            if (response.StatusCode == (int) HttpStatusCode.OK)
            {
                json = response.Content;
            }

            var roundResult = JsonConvert.DeserializeObject<RoundResult>(json);
            PrintResult(roundResult);
        }

        public void StartGame(HttpClient client, GamerInfo[] gamers, string currentUser, int roomId)
        {
            while (true)
            {
                var seriesStopWatch = new Stopwatch();
                seriesStopWatch.Start();

                while (true)
                {
                    if (seriesStopWatch.Elapsed.Minutes > 5)
                    {
                        RoomRequests.SaveStatRounds(client, roomId);
                        RoomRequests.DeleteRoom(client, roomId);
                        break;
                    }

                    var round = StartRound(client, gamers, currentUser, roomId);
                    if (round != null)
                    {
                        RoundRequests.AddRoundToRoom(client, round, roomId);
                        seriesStopWatch.Restart();
                    }

                    break;
                }

                if (WantToContinue(client, roomId) == false)
                {
                    return;
                }

                do
                {
                    
                } while (RoundRequests.Put(client, roomId, client.BaseAddress + $"api/rounds/ready/check/{roomId}") == false);
                
                RoundRequests.Put(client, roomId, client.BaseAddress + $"api/rounds/refresh/{roomId}");
            }
        }

        private bool WantToContinue(HttpClient client, int roomId)
        {
            Console.WriteLine("If you want to continue play enter 0,");
            Console.WriteLine("another number for leave from the room:");
            var input = Console.ReadLine();

            if (input == "0")
            {
                RoundRequests.Put(client, roomId, client.BaseAddress + $"api/rounds/ready/{roomId}");
                return true;
            }

            RoomRequests.SaveStatRounds(client, roomId);
            RoomRequests.DeleteRoom(client, roomId);
            return false;
        }

        private Round StartRound(HttpClient client, GamerInfo[] gamers, string currentUser, int roomId)
        {
            while (true)
            {
                var roundTaskResult = GetRound(client, gamers, currentUser, roomId);
                if (roundTaskResult == null || !roundTaskResult.IsValid())
                {
                    Console.WriteLine("Round was canceled!");
                    RoundRequests.Put(client, roomId, client.BaseAddress + $"api/rounds/refresh/{roomId}");
                    return null;
                }
                
                PrintResult(GetCorrectResultByUserName(roundTaskResult, currentUser));
                return roundTaskResult;
            }
        }

        private Round GetRound(HttpClient client, GamerInfo[] gamers, string currentUser, int roomId)
        {
            Round round;

            var roundConfigurationSw = new Stopwatch();
            var requestPeriodSw = new Stopwatch();
            
            requestPeriodSw.Start();
            roundConfigurationSw.Start();
            
            var cancelTokenSource = new CancellationTokenSource();
            
            var task = Task.Run(() => UserActionInRound(client, gamers, currentUser, roomId), cancelTokenSource.Token);

            while (true)
            {
                if (requestPeriodSw.ElapsedMilliseconds > 1000)
                {
                    round = RoomRequests.GetRound(client, client.BaseAddress + $"api/rounds/complete/{roomId}");
                    if (round != null)
                    {
                        if (roundConfigurationSw.Elapsed.Seconds < 20)
                        {
                            if (round.UserAction1 == GameActions.None || round.UserAction2 == GameActions.None)
                            {
                                requestPeriodSw.Restart();
                                continue;
                            }
                        }
                        
                        if (!task.IsCompleted)
                        {
                            cancelTokenSource.Cancel();
                            return null;
                        }
                        
                        if(round.IsValid() == false)
                        {
                            return null;
                        }
                        
                        break;
                    }

                    if (roundConfigurationSw.Elapsed.Seconds > 20)
                    {
                        if (!task.IsCompleted)
                        {
                            cancelTokenSource.Cancel();
                            return null;
                        }
                        
                        break;
                    }

                    requestPeriodSw.Restart();
                }
            }

            return round;
        }

        private void UserActionInRound(HttpClient client, GamerInfo[] gamers, string currentUser, int roomId)
        {
            GamerInfo current;
            GamerInfo opponent;
            if (currentUser == gamers[0].UserName)
            {
                current = gamers[0];
                opponent = gamers[1];
            }
            else
            {
                current = gamers[1];
                opponent = gamers[0];
            }
            
            Console.WriteLine($"Room ID:\t{roomId}");
            Console.WriteLine($"Match:\t{current.UserName} vs {opponent.UserName}");
            Console.WriteLine("Rules:\tRock > scissors; scissors > paper; paper > rock.\n");

            var action = GetAction();
            if (action == GameActionsUi.None)
            {
                if(RoomRequests.DeleteGamer(client, current, roomId))
                {
                    Console.WriteLine("You leave from the game!");
                    return;
                }
            }

            GameRequests.GameAction(client, current, action, roomId);
        }

        private GameActionsUi GetAction()
        {
            Console.WriteLine("1.\tRock");
            Console.WriteLine("2.\tScissors");
            Console.WriteLine("3.\tPaper");
            

            Console.WriteLine();
            var action = GetNumberFromUser("Enter the number: ");
            switch (action)
            {
                case 1:
                    return GameActionsUi.Rock;
                case 2:
                    return GameActionsUi.Scissors;
                case 3:
                    return GameActionsUi.Paper;
            }

            return GameActionsUi.None;
        }

        private RoundResult GetCorrectResultByUserName(Round round, string userName)
        {
            return userName == round.Gamer1.UserName
                ? round.RoundResultForGamer1
                : round.RoundResultForGamer2;
        }

        private void PrintResult(RoundResult result)
        {
            switch (result)
            {
                case RoundResult.Draw:
                    Console.WriteLine("Draw!");
                    break;
                case RoundResult.Win:
                    Console.WriteLine("You Win!");
                    break;
                case RoundResult.Lose:
                    Console.WriteLine("You Lose!");
                    break;
                case RoundResult.None:
                    Console.WriteLine("Round was canceled!");
                    break;
            }
        }

        private int GetNumberFromUser(string message)
        {
            while (true)
            {
                Console.WriteLine(message);
                var input = Console.ReadLine();
                if (int.TryParse(input, out var number) && number > 0 && number < 4)
                {
                    return number;
                }

                Console.WriteLine("Incorrect number. Try again");
            }
        }
    }
}
