using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RSPGame.UI.PlayRequests
{
    public static class GameRequests
    {
        public static HttpResponseMessage RequestWithTimer(HttpClient client, string path, int seconds)
        {
            var counter = 0;
            HttpResponseMessage response;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (true)
            {
                if (stopwatch.ElapsedMilliseconds < 1000) continue;
                try
                {
                    response = client.GetAsync(path).Result;
                }
                catch (HttpRequestException)
                {
                    return null;
                }
                catch (NullReferenceException)
                {
                    return null;
                }

                if (response.StatusCode == HttpStatusCode.OK) break;

                counter++;
                stopwatch.Restart();
                if (counter == seconds) break;
            }

            return response;
        }

        public static async Task<IEnumerable<string>> GetGame(HttpClient client, int roomId)
        {
            if (client == null)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }

            var response = RequestWithTimer(client, $"api/rooms/{roomId}", 30);
            if (response == null)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("\nThe game could not be found. Please try again.\n\n");
                await client.DeleteAsync($"api/rooms/stop/{roomId}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string[]>(json);
        }

    }
}
