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
        public void PlayWithBot(HttpClient client, string token)
        {
            var action = GetAction();

            var json = JsonConvert.SerializeObject(action);
            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + "api/game/bot",
                Method = RequestMethod.Post,
                Body = json,
                Token = token
            };
            
            var response = RequestHandler.HandleRequest(client, requestOptions);
            if(response == null)
                return;
            
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

        public void StartGame(HttpClient client, GamerInfo[] gamers, Session session, int roomId)
        {
            while (true)
            {
                var seriesStopWatch = new Stopwatch();
                seriesStopWatch.Start();

                while (true)
                {
                    if (seriesStopWatch.Elapsed.Minutes > 5)
                    {
                        RoomRequests.SaveStatRounds(client, session.Token, roomId);
                        RoomRequests.DeleteRoom(client, session.Token, roomId);
                        break;
                    }

                    var round = StartRound(client, gamers, session, roomId);
                    if (round != null)
                    {
                        RoundRequests.AddRoundToRoom(client, round, session.Token, roomId);
                        seriesStopWatch.Restart();
                    }

                    break;
                }

                if (WantToContinue(client, session.Token, roomId) == false)
                {
                    Console.WriteLine("Game over\n");
                    return;
                }
                
                RoundRequests.Put(client, session.Token,  client.BaseAddress + $"api/rounds/ready/{roomId}");

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                int counter = 0;
                
                while (true)
                {
                    if (stopWatch.ElapsedMilliseconds > 1000)
                    {
                        if (RoundRequests.Put(client, session.Token, client.BaseAddress + $"api/rounds/ready/check/{roomId}"))
                        {
                            break;
                        }
                        
                        counter++;
                        stopWatch.Restart();
                    }

                    if (counter == 10)
                    {
                        Console.WriteLine("Gamer over!\n");
                        return;
                    }
                }
                
                RoundRequests.Put(client, session.Token, client.BaseAddress + $"api/rounds/refresh/{roomId}");
            }
        }

        private bool WantToContinue(HttpClient client, string token, int roomId)
        {
            Console.WriteLine("If you want to continue play enter 0,");
            Console.WriteLine("another number for leave from the room:");
            var input = Console.ReadLine();

            if (input == "0")
            {
                return true;
            }

            RoomRequests.SaveStatRounds(client, token, roomId);
            RoomRequests.DeleteRoom(client, token, roomId);
            return false;
        }

        private Round StartRound(HttpClient client, GamerInfo[] gamers, Session session, int roomId)
        {
            while (true)
            {
                var roundTaskResult = GetRound(client, gamers, session, roomId);
                if (roundTaskResult == null || !roundTaskResult.IsValid())
                {
                    Console.WriteLine("Round was canceled!");
                    RoundRequests.Put(client, session.Token, client.BaseAddress + $"api/rounds/refresh/{roomId}");
                    return null;
                }
                
                PrintResult(GetCorrectResultByUserName(roundTaskResult, session.UserName));
                return roundTaskResult;
            }
        }

        private Round GetRound(HttpClient client, GamerInfo[] gamers, Session session, int roomId)
        {
            Round round;

            var roundConfigurationSw = new Stopwatch();
            var requestPeriodSw = new Stopwatch();
            
            requestPeriodSw.Start();
            roundConfigurationSw.Start();
            
            var cancelTokenSource = new CancellationTokenSource();
            
            var task = Task.Run(() => UserActionInRound(client, gamers, session, roomId), cancelTokenSource.Token);

            while (true)
            {
                if (requestPeriodSw.ElapsedMilliseconds > 1000)
                {
                    round = RoomRequests.GetRound(client, session.Token, client.BaseAddress + $"api/rounds/complete/{roomId}");
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

        private void UserActionInRound(HttpClient client, GamerInfo[] gamers, Session session, int roomId)
        {
            GamerInfo current;
            GamerInfo opponent;
            if (session.UserName == gamers[0].UserName)
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
                if(RoomRequests.DeleteGamer(client, session, roomId))
                {
                    Console.WriteLine("You leave from the game!");
                    return;
                }
            }

            GameRequests.GameAction(client, session, action, roomId);
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
                    Console.WriteLine("\nDraw!\n");
                    break;
                case RoundResult.Win:
                    Console.WriteLine("\nYou Win!\n");
                    break;
                case RoundResult.Lose:
                    Console.WriteLine("\nYou Lose!\n");
                    break;
                case RoundResult.None:
                    Console.WriteLine("\nRound was canceled!\n");
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

                Console.WriteLine("\nIncorrect number. Try again\n");
            }
        }
    }
}
