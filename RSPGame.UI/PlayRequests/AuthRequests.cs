using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.UI.Menus;

namespace RSPGame.UI.PlayRequests
{
    public static class AuthRequests
    {
        public static async Task Register(HttpClient client, Session currentSession)
        {
            Console.WriteLine("Registration");

            var response = await GetResponse(client, "api/auth/register");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonFromApi = await response.Content.ReadAsStringAsync();

                currentSession = JsonSerializer.Deserialize<Session>(jsonFromApi);
                
                await new SessionMenu(client, currentSession).Start();
                return;
            }

            Console.WriteLine(response.StatusCode == HttpStatusCode.BadRequest
                ? "Invalid register values!"
                : "Account do not created!");
        }
        
        public static async Task Login(HttpClient client, Session currentSession, Stopwatch stopwatch)
        {
            Console.WriteLine("Login");

            var response = await GetResponse(client, "api/auth/login");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonFromApi = await response.Content.ReadAsStringAsync();

                currentSession = JsonSerializer.Deserialize<Session>(jsonFromApi);
                
                await new SessionMenu(client, currentSession).Start();
                return;
            }
            
            currentSession.CountLoginFailed++;

            Console.WriteLine(response.StatusCode == HttpStatusCode.BadRequest
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
        
        public static async Task<HttpResponseMessage> GetResponse(HttpClient client, string url)
        {
            var userName = GetStringFromUser("Enter your user name:");
            var password = GetStringFromUser("Enter your password:");
                
            var uri = client.BaseAddress + url;

            var user = new RequestUser
            {
                UserName = userName,
                Password = password
            };

            var userJson = JsonSerializer.Serialize(user);

            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            return await client.PostAsync(uri, content);
        }
    }
}