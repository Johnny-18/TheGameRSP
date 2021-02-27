using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.Models.Game;
using RSPGame.UI.Models;

namespace RSPGame.UI.PlayRequests
{
    public static class GameRequests
    {
        public static GamerInfo[] GetGame(HttpClient client, int roomId, int count)
        {
            if (client == null)
                return null;

            var counter = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/rooms/gamers/{roomId}",
                Method = RequestMethod.Get
            };

            while (true)
            {
                if (stopwatch.ElapsedMilliseconds < 1000) 
                    continue;

                try
                {
                    var response = RequestHandler.HandleRequest(client, requestOptions);
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
                        //Console.WriteLine("\nThe game could not be found. Please try again.\n\n");
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

        public static async Task<string> PostAction(HttpClient client, GamerInfo gamerInfo, GameActionsUi action)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var gameRequest = new GameRequest
            {
                GamerInfo = gamerInfo,
                Action = (GameActions)action
            };
            
            var json = JsonConvert.SerializeObject(gameRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            try
            {
                //todo
                var message = await client.PostAsync($"/api/round", content);
                if (message.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("\nRequest has been sent!\n\n");
                    return await message.Content.ReadAsStringAsync();
                }
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }

            return null;
        }

    }
}
