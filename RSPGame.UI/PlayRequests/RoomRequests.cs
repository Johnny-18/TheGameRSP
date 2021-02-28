using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.Models.Game;
using RSPGame.UI.Models;

namespace RSPGame.UI.PlayRequests
{
    public static class RoomRequests
    {
        public static void DeleteRoom(HttpClient client, string token, int roomId)
        {
            if (client == null)
                return;
            
            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/rooms/{roomId}",
                Method = RequestMethod.Delete,
                Token = token
            };

            var response = RequestHandler.HandleRequest(client, requestOptions);
            if (response == null)
                return;
            
            if (response.StatusCode == (int)HttpStatusCode.BadRequest || response.StatusCode == (int)HttpStatusCode.NoContent)
            {
                Console.WriteLine("\nSomething going wrong!\n");
            }
        }

        public static Round GetRound(HttpClient client, string token, string url)
        {
            if (client == null)
                return null;
            
            var requestOptions = new RequestOptions
            {
                Address = url,
                Method = RequestMethod.Get,
                Token = token
            };
            
            var response = RequestHandler.HandleRequest(client, requestOptions);
            if (response == null)
                return null;
            
            if (response.StatusCode == (int)HttpStatusCode.NotFound || response.StatusCode == (int)HttpStatusCode.NoContent)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Round>(response.Content);
        }

        public static bool JoinRoom(HttpClient client, Session session, int id)
        {
            if (client == null || session?.GamerInfo == null)
                return false;

            var json = JsonConvert.SerializeObject(session.GamerInfo);

            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/rooms/join?id={id}",
                Body = json,
                Method = RequestMethod.Post,
                Token = session.Token
            };
            
            try
            {
                var response = RequestHandler.HandleRequest(client, requestOptions);
                if (response == null)
                    return false;
                
                if (response.StatusCode == (int) HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("You need to login! Or register your account!");
                    return false;
                }
                
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

        public static bool DeleteGamer(HttpClient client, Session session,int roomId)
        {
            if (client == null || session?.GamerInfo == null)
                return false;

            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/rooms/gamer/{roomId}",
                Method = RequestMethod.Delete,
                Body = JsonConvert.SerializeObject(session.GamerInfo),
                Token = session.Token
            };

            var response = RequestHandler.HandleRequest(client, requestOptions);
            if (response == null)
                return false;
            
            if (response.StatusCode != (int) HttpStatusCode.OK)
            {
                return false;
            }

            return true;
        }

        public static void SaveStatRounds(HttpClient client, string token, int roomId)
        {
            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/rooms/save/{roomId}",
                Method = RequestMethod.Get,
                Token = token
            };
            
            RequestHandler.HandleRequest(client, requestOptions);
        }
    }
}
