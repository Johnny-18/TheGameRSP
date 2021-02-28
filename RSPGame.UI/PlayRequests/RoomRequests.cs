using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models.GameModel;

namespace RSPGame.UI.PlayRequests
{
    public static class RoomRequests
    {
        public static Task<string> PostAsync(HttpClient client, GamerInfo gamer, string path)
        {
            if (client == null || gamer == null)
                return null;

            var json = JsonConvert.SerializeObject(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var message = client.PostAsync($"/api/rooms/{path}", content).Result;
                return message.Content.ReadAsStringAsync();
            }
            catch (AggregateException)
            {
                return null;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public static Task JoinAsync(HttpClient client, GamerInfo gamer, int id)
        {
            if (client == null || gamer == null)
                return null;

            var json = JsonConvert.SerializeObject(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage message;

            try
            {
                message = client.PostAsync($"/api/rooms/join?id={id}", content).Result;
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }

            if (message.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("\nThe room was not found. Check the number again.\n\n");
                return null;
            }

            return Task.CompletedTask;
        }
        
        // public static async Task<bool> DeleteGamer(HttpClient client, string userName,int roomId)
        // {
        //     if (client == null)
        //         return false;
        //
        //     var json = JsonConvert.SerializeObject(userName);
        //
        //     var content = new StringContent(json, Encoding.UTF8, "application/json");
        //
        //     var response = await client.PostAsync($"api/rooms/gamer/{roomId}", content);
        //     if (response.StatusCode != HttpStatusCode.OK)
        //     {
        //         return false;
        //     }
        //
        //     return true;
        // }
    }
}
