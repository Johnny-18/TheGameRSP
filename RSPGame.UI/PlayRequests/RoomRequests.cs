using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.Models.Game;
using RSPGame.Models.RoomModel;
using RSPGame.UI.Game;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RSPGame.UI.PlayRequests
{
    public static class RoomRequests
    {
        public static async Task<string> QuickSearch(HttpClient client, GamerInfo gamer)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var json = JsonSerializer.Serialize(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            try
            {
                var message = await client.PostAsync($"/api/rooms/find", content);
                return await message.Content.ReadAsStringAsync();
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }
        }

        public static async Task<string> CreateRoom(HttpClient client, GamerInfo gamer)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var json = JsonSerializer.Serialize(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var message = await client.PostAsync($"/api/rooms/create", content);
                return await message.Content.ReadAsStringAsync();
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }
        }

        public static async Task<GamerInfo[]> GetGamers(HttpClient client, int roomId)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            try
            {
                var response = await client.GetAsync($"/api/rooms/{roomId}");
                if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    Console.WriteLine("\nThe room was not found. Check the number again.\n\n");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var roomRepository = JsonConvert.DeserializeObject<RoomRepository>(json);
                if (roomRepository != null && roomRepository.IsStarted)
                {
                    return roomRepository.GetGamers().ToArray();
                }

                return null;
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }
        }

        public static async Task<bool> DeleteRoom(HttpClient client, int roomId)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            
            try
            {
                var response = await client.DeleteAsync($"/api/rooms/{roomId}");
                if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NoContent)
                {
                    Console.WriteLine("\nSomething going wrong!\n\n");
                    return false;
                }

                return true;
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return false;
            }
        }

        public static async Task GameAction(HttpClient client, GamerInfo gamer, GameActionsUi action, int roomId)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var gameRequest = new GameRequest
            {
                GamerInfo = gamer,
                Action = (GameActions)action
            };

            var json = JsonConvert.SerializeObject(gameRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            try
            {
                var response = await client.PostAsync($"/api/rooms/{roomId}/action", content);
                
                if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    Console.WriteLine("\nThe room was not found. Check the number again.\n\n");
                    return;
                }

                Console.WriteLine("\nWaiting end of round!\n\n");
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
            }
        }

        public static async Task<Round> GetLastRound(HttpClient client, int roomId)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var response = await client.GetAsync($"{roomId}/lastRound");
            
            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.NoContent)
            {
                Console.WriteLine("\nRound was failed!\n\n");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var round = JsonConvert.DeserializeObject<Round>(json);

            return round;
        }

        public static async Task<bool> JoinRoom(HttpClient client, GamerInfo gamer, int id)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var json = JsonSerializer.Serialize(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"/api/rooms/join?id={id}", content);
                
                if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    Console.WriteLine("\nThe room was not found. Check the number again.\n\n");
                    return false;
                }

                Console.WriteLine("\nDone!\n\n");
                return true;
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return false;
            }
        }

        public static async Task<RoomRepository> GetRoomById(HttpClient client, int roomId)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var response = await client.GetAsync($"api/rooms/{roomId}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<RoomRepository>(json);
            }

            return null;
        }
    }
}
