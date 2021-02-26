using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.Models.Game;
using RSPGame.UI.Game;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RSPGame.UI.PlayRequests
{
    public static class GameRequests
    {
        public static async Task<GamerInfo[]> GetGame(HttpClient client, int roomId)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var counter = 0;
            HttpResponseMessage response;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (true)
            {
                if (stopwatch.ElapsedMilliseconds < 2500) 
                    continue;

                try
                {
                    response = await client.GetAsync($"api/rooms/gamers/{roomId}");
                    
                    if (response.StatusCode == HttpStatusCode.OK) 
                        break;

                    counter++;
                    stopwatch.Restart();
                    if (counter == 12) 
                        break;
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                    return null;
                }
            }

            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest)
            {
                Console.WriteLine("\nThe game could not be found. Please try again.\n\n");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var gamers = JsonConvert.DeserializeObject<GamerInfo[]>(json);
            return gamers;
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
            
            var json = JsonSerializer.Serialize(gameRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            try
            {
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
