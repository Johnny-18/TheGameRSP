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
        public async void StartGame(HttpClient client, GamerInfo[] gamers, string currentUser, int roomId)
        {
            
            //todo start series
            
            Round round;
            var roundConfigurationSw = new Stopwatch();
            var requestPeriodSw = new Stopwatch();
            
            requestPeriodSw.Start();
            roundConfigurationSw.Start();
            
            var cancelTokenSource = new CancellationTokenSource();
            var token = cancelTokenSource.Token;
            
            var task = Task.Run(() => StartRound(client, gamers, roomId), token);
            //StartRound(client, gamers, roomId);

            while (true)
            {
                if (requestPeriodSw.ElapsedMilliseconds > 500)
                {
                    round = RoomRequests.GetLastRound(client, roomId);
                    if (round != null)
                    {
                        if (roundConfigurationSw.Elapsed.Seconds < 20)
                        {
                            if (round.UserAction1 == GameActions.None || round.UserAction2 == GameActions.None)
                            {
                                requestPeriodSw.Restart();
                                continue;
                            }

                            PrintResult(GetCorrectResultByUserName(round, currentUser));
                            break;
                        }
                        
                        if(!task.IsCompleted)
                            cancelTokenSource.Cancel();
                        
                        //todo remove invalid round

                        PrintResult(GetCorrectResultByUserName(round, currentUser));
                        break;
                    }
                    
                    requestPeriodSw.Restart();
                }
            }

            var roomRep = await RoomRequests.GetRoomById(client, roomId);
            
            roomRep.SeriesRepository.AddRound(round);
        }

        public void PlayWithBotAsync(HttpClient client)
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
            if (response.StatusCode == (int) HttpStatusCode.OK)
            {
                json = response.Content;
            }

            var roundResult = JsonConvert.DeserializeObject<RoundResult>(json);
            PrintResult(roundResult);
        }

        private void StartRound(HttpClient client, GamerInfo[] gamers, int roomId)
        {
            var firstGamer = gamers[0];
            var secondGamer = gamers[1];
            
            Console.Clear();
            Console.WriteLine($"Room ID:\t{roomId}");
            Console.WriteLine($"Match:\t{firstGamer.UserName} vs {secondGamer.UserName}");
            Console.WriteLine("Rules:\tRock > scissors; scissors > paper; paper > rock.\n");

            var action = GetAction();

            RoomRequests.GameAction(client, firstGamer, action, roomId);
        }

        private GameActionsUi GetAction()
        {
            Console.WriteLine("1.\tRock");
            Console.WriteLine("2.\tScissors");
            Console.WriteLine("3.\tPaper");
            Console.WriteLine("4.\tExit");
            

            Console.WriteLine();
            switch (GetNumberFromUser("Enter the number: "))
            {
                case 1:
                    return GameActionsUi.Rock;
                case 2:
                    return GameActionsUi.Scissors;
                case 3:
                    return GameActionsUi.Paper;
                default:
                    Console.WriteLine("Goodbye!");
                    return GameActionsUi.None;
            }
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
                if (int.TryParse(Console.ReadLine(), out var number) || number > 0 && number < 5)
                {
                    return number;
                }

                Console.WriteLine("Incorrect number. Try again");
            }
        }
    }
}
