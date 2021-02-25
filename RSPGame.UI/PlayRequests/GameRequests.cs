using RSPGame.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RSPGame.UI.PlayRequests
{
    public static class GameRequests
    {
        public static async Task<string> GetGame(HttpClient client, int roomId)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            string result;

            try
            {
                var response = await client.GetAsync($"api/game/{roomId}");
                result = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }

            return result;
        }
    }
}
