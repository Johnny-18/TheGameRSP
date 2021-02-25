using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RSPGame.Models;

namespace RSPGame.UI.PlayRequests
{
    public static class RoomRequests
    {
        public static Task<string> QuickSearch(HttpClient client, GamerInfo gamer)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var json = JsonSerializer.Serialize(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Task<string> result;

            try
            {
                var message = client.PostAsync($"/api/rooms/find", content).Result;
                result = message.Content.ReadAsStringAsync();
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }

            return result;
        }

        public static Task<string> CreateRoom(HttpClient client, GamerInfo gamer)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var json = JsonSerializer.Serialize(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Task<string> result;

            try
            {
                var message = client.PostAsync($"/api/rooms/create", content).Result;
                result = message.Content.ReadAsStringAsync();
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }

            return result;
        }

        public static Task JoinRoom(HttpClient client, GamerInfo gamer, int id)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var json = JsonSerializer.Serialize(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage message;

            try
            {
                message = client.PostAsync($"/api/rooms/join?id={id}", content).Result;
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return Task.CompletedTask;
            }

            if (message.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("\nThe room was not found. Check the number again.\n\n");
                return Task.CompletedTask;
            }

            Console.WriteLine("\nDone!\n\n");
            return Task.CompletedTask;
        }
    }
}
