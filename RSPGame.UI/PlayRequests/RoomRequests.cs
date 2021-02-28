using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.Models.Game;
using RSPGame.Services.Rooms;
using RSPGame.UI.Models;

namespace RSPGame.UI.PlayRequests
{
    public static class RoomRequests
    {
        public static GamerInfo[] GetGamers(HttpClient client, int roomId)
        {
            if (client == null)
                return null;

            try
            {
                var requestOptions = new RequestOptions
                {
                    Address = client.BaseAddress + $"/api/rooms/{roomId}",
                    Method = RequestMethod.Get
                };
                
                var response = RequestHandler.HandleRequest(client, requestOptions);
                if (response.StatusCode == (int)HttpStatusCode.NotFound || response.StatusCode == (int)HttpStatusCode.BadRequest)
                {
                    //Console.WriteLine("\nThe room was not found. Check the number again.\n\n");
                    return null;
                }

                var json = response.Content;
                
                var roomRepository = JsonConvert.DeserializeObject<RoomRepository>(json);
                
                return roomRepository?.GetGamers().ToArray();
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
            
            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"/api/rooms/{roomId}",
                Method = RequestMethod.Delete
            };

            var response = RequestHandler.HandleRequest(client, requestOptions);
            if (response.StatusCode == (int)HttpStatusCode.BadRequest || response.StatusCode == (int)HttpStatusCode.NoContent)
            {
                Console.WriteLine("\nSomething going wrong!\n\n");
            }
        }

        public static Round GetRound(HttpClient client, string url)
        {
            if (client == null)
                return null;
            
            var requestOptions = new RequestOptions
            {
                Address = url,
                Method = RequestMethod.Get
            };
            
            var response = RequestHandler.HandleRequest(client, requestOptions);
            if (response.StatusCode == (int)HttpStatusCode.NotFound || response.StatusCode == (int)HttpStatusCode.NoContent)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Round>(response.Content);
        }

        public static bool JoinRoom(HttpClient client, GamerInfo gamer, int id)
        {
            if (client == null || gamer == null)
                return false;

            var json = JsonConvert.SerializeObject(gamer);

            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/rooms/join?id={id}",
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

        public static bool DeleteGamer(HttpClient client, GamerInfo currentUser, int roomId)
        {
            if (client == null || currentUser == null)
                return false;

            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/rooms/gamer/{roomId}",
                Method = RequestMethod.Delete,
                Body = JsonConvert.SerializeObject(currentUser)
            };

            var response = RequestHandler.HandleRequest(client, requestOptions);
            if (response.StatusCode != (int) HttpStatusCode.OK)
            {
                return false;
            }

            return true;
        }

        public static void SaveStatRounds(HttpClient client, int roomId)
        {
            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/rooms/save/{roomId}",
                Method = RequestMethod.Get
            };
            
            RequestHandler.HandleRequest(client, requestOptions);
        }
    }
}
