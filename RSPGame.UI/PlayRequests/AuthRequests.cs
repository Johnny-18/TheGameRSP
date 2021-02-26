using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.UI.Menus;
using RSPGame.UI.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RSPGame.UI.PlayRequests
{
    public static class AuthRequests
    {
        public static async Task Register(HttpClient client, Session currentSession)
        {
            Console.WriteLine("Registration");

            var response = GetResponse(client, "api/auth/register");
            if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                var jsonFromApi = response.Content;

                currentSession = JsonSerializer.Deserialize<Session>(jsonFromApi);
                
                await new SessionMenu(client, currentSession).Start();
                return;
            }

            Console.WriteLine(response.StatusCode == (int)HttpStatusCode.BadRequest
                ? "Invalid register values!"
                : "Account do not created!");
        }
        
        public static async Task Login(HttpClient client, Session currentSession, Stopwatch stopwatch)
        {
            Console.WriteLine("Login");

            var response = GetResponse(client, "api/auth/login");
            if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                var jsonFromApi = response.Content;

                currentSession = JsonConvert.DeserializeObject<Session>(jsonFromApi);
                
                await new SessionMenu(client, currentSession).Start();
                return;
            }
            currentSession.CountLoginFailed++;

            Console.WriteLine(response.StatusCode == (int)HttpStatusCode.BadRequest
                ? "Invalid login values!"
                : "Account do not found!");

            if (currentSession.CountLoginFailed == 3)
            {
                stopwatch.Start();
            }
        }

        public static string GetStringFromUser(string message)
        {
            while (true)
            {
                Console.WriteLine(message);
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input) || input.Length < 6)
                {
                    Console.WriteLine("Invalid string! Length 6 and more symbols!");
                    continue;
                }

                return input;
            }
        }

        public static Response GetResponse(HttpClient client, string url)
        {
            var user = UserDataFromConsole();
            var userJsonForBody = JsonSerializer.Serialize(user);

            var requestOptions = new RequestOptions
            {   
                Method = RequestMethod.Post,
                Address = client.BaseAddress + url,
                Body = userJsonForBody
            };

            return RequestHandler.HandleRequest(client, requestOptions);
        }

        private static RequestUser UserDataFromConsole()
        {
            var userName = GetStringFromUser("Enter your user name:");
            var password = GetStringFromUser("Enter your password:");
            
            return new RequestUser
            {
                UserName = userName,
                Password = password
            };
        }
    }
}