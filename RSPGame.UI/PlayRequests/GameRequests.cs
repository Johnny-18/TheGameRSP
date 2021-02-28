using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.Models.Game;
using RSPGame.UI.Models;

namespace RSPGame.UI.PlayRequests
{
    public static class GameRequests
    {
        public static GamerInfo[] GetGamers(HttpClient client, string token, int roomId, int count)
        {
            if (client == null)
                return null;

            var counter = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/rooms/gamers/{roomId}",
                Method = RequestMethod.Get,
                Token = token
            };

            while (true)
            {
                if (stopwatch.ElapsedMilliseconds < 1000) 
                    continue;

                try
                {
                    var response = RequestHandler.HandleRequest(client, requestOptions);
                    if (response == null)
                        return null;
                    
                    if (response.StatusCode == (int) HttpStatusCode.OK)
                    {
                        var json = response.Content;
                        var gamers = JsonConvert.DeserializeObject<GamerInfo[]>(json);
                        if(gamers != null && gamers.Length == 2)
                            return gamers;
                    }

                    counter++;
                    stopwatch.Restart();
                    if (counter == count)
                    {
                        return null;
                    }
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                    return null;
                }
            }
        }
        
        public static void GameAction(HttpClient client, Session session, GameActionsUi action, int roomId)
        {
            if (client == null || session == null || session.GamerInfo == null)
                return;

            var gameRequest = new GameRequest
            {
                GamerInfo = session.GamerInfo,
                Action = (GameActions)action
            };

            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/rounds/action/{roomId}",
                Method = RequestMethod.Post,
                Body = JsonConvert.SerializeObject(gameRequest),
                Token = session.Token
            };

            var response = RequestHandler.HandleRequest(client, requestOptions);
            if (response == null)
                return;
            
            if (response.StatusCode == (int)HttpStatusCode.NotFound || response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                Console.WriteLine("\nSomething going wrong!.\n\n");
            }

            Console.WriteLine($"Your choice: {action.ToString()}");
        }
    }
}
