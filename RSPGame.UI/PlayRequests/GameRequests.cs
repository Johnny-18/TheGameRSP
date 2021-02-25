using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace RSPGame.UI.PlayRequests
{
    public static class GameRequests
    {
        public static IEnumerable<string> GetGame(HttpClient client, int roomId)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var counter = 0;
            HttpResponseMessage response;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            while (true)
            {
                if (stopwatch.ElapsedMilliseconds < 2500) continue;

                try
                {
                    response = client.GetAsync($"api/game/{roomId}").Result;
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                    return null;
                }
                if (response.StatusCode == HttpStatusCode.OK) break;

                counter++;
                stopwatch.Restart();
                if (counter == 12) break;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("\nThe game could not be found. Please try again.\n\n");
                return null;
            }

            Console.WriteLine("\nDone!\n\n");
            
            var json = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<string[]>(json);
        }

    }
}
