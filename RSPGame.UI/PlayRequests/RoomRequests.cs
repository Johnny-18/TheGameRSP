using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RSPGame.Models;

namespace RSPGame.UI.PlayRequests
{
    public static class RoomRequests
    {
        public static Task QuickSearch(HttpClient client, GamerInfo gamer)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var json = JsonSerializer.Serialize(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var task = Task.Run(() => client.PostAsync($"/api/rooms/find", content));
                task.Wait();
            }
            catch (AggregateException e)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return Task.FromException(e);
            }

            Console.WriteLine("\nWaiting for opponent\n\n");
            return Task.CompletedTask;
        }

        public static Task<string> CreateRoom(HttpClient client, GamerInfo gamer)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var json = JsonSerializer.Serialize(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task;

            try
            {
                task = Task.Run(() => client.PostAsync($"/api/rooms/create", content));
                task.Wait();
            }
            catch (AggregateException e)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return (Task<string>)Task.FromException(e);
            }

            var result = task.Result.Content.ReadAsStringAsync();
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

            try
            {
                var task = Task.Run(() => client.PostAsync($"/api/rooms/join?id={id}", content));
                task.Wait();
            }
            catch (AggregateException e)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return Task.FromException(e);
            }

            Console.WriteLine("\nDone!\n\n");
            return Task.CompletedTask;
        }
    }
}
