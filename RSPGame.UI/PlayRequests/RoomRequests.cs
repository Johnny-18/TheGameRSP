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
using RSPGame.Services.Rooms;
using RSPGame.UI.Game;
using RSPGame.UI.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RSPGame.UI.PlayRequests
{
    public static class RoomRequests
    {
        public static string Post(HttpClient client, RequestOptions requestOptions)
        {
            if (requestOptions == null)
                return null;
            
            try
            {
                var response = RequestHandler.HandleRequest(client, requestOptions);
                return response.Content;
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }
        }

        public static GamerInfo[] GetGamers(HttpClient client, int roomId)
        {
            if (client == null)
                return null;

            try
            {
                var requestOptions = new RequestOptions
                {
                    Address = $"/api/rooms/{roomId}",
                    Method = RequestMethod.Get
                };
                
                var response = RequestHandler.HandleRequest(client, requestOptions);
                if (response.StatusCode == (int)HttpStatusCode.NotFound || response.StatusCode == (int)HttpStatusCode.BadRequest)
                {
                    Console.WriteLine("\nThe room was not found. Check the number again.\n\n");
                    return null;
                }

                var json = response.Content;
                
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

        public static void DeleteRoom(HttpClient client, int roomId)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            
            try
            {
                var requestOptions = new RequestOptions
                {
                    Address = $"/api/rooms/{roomId}",
                    Method = RequestMethod.Delete
                };

                var response = RequestHandler.HandleRequest(client, requestOptions);
                if (response.StatusCode == (int)HttpStatusCode.BadRequest || response.StatusCode == (int)HttpStatusCode.NoContent)
                {
                    Console.WriteLine("\nSomething going wrong!\n\n");
                }
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
            }
        }

        public static void GameAction(HttpClient client, GamerInfo gamer, GameActionsUi action, int roomId)
        {
            if (client == null || gamer == null)
                return;

            var gameRequest = new GameRequest
            {
                GamerInfo = gamer,
                Action = (GameActions)action
            };

            var json = JsonConvert.SerializeObject(gameRequest);

            var requestOptions = new RequestOptions
            {
                Address = $"/api/rooms/{roomId}/action",
                Method = RequestMethod.Post,
                Body = json
            };
            
            try
            {
                var response = RequestHandler.HandleRequest(client, requestOptions);
                
                if (response.StatusCode == (int)HttpStatusCode.NotFound || response.StatusCode == (int)HttpStatusCode.BadRequest)
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

        public static bool JoinRoom(HttpClient client, GamerInfo gamer, int id)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var json = JsonSerializer.Serialize(gamer);

            var requestOptions = new RequestOptions
            {
                Address = $"/api/rooms/join?id={id}",
                Body = json,
                Method = RequestMethod.Post
            };
            
            try
            {
                var response = RequestHandler.HandleRequest(client, requestOptions);
                if (response.StatusCode != (int)HttpStatusCode.OK)
                {
                    Console.WriteLine("\nThe room was not found. Check the number again.\n\n");
                    return false;
                }

                var roomId = JsonConvert.DeserializeObject<int>(response.Content);
                
                Console.WriteLine($"\nRoom was found {roomId}!");
                Console.WriteLine("Waiting for opponent!\n\n");
                
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
