using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RSPGame.UI.PlayRequests
{
    public static class GameRequests
    {
        public static HttpResponseMessage GetGame(HttpClient client, int roomId)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            HttpResponseMessage response;

            try
            {
                response = client.GetAsync($"api/game/{roomId}").Result;
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }

            return response;
        }
    }
}
