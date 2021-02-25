using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RSPGame.Models;

namespace RSPGame.UI.Menus
{
    public class AuthorizationMenu
    {
        private Session _currentSession;

        private readonly HttpClient _client;

        private readonly Stopwatch _stopwatch;

        public AuthorizationMenu(HttpClient client, Session currentSession)
        {
            _client = client;
            _currentSession = currentSession;
            _stopwatch = new Stopwatch();
        }

        public async Task Start()
        {
            Console.Clear();
            
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Start menu");
                Console.WriteLine("1.\tRegistration");
                Console.WriteLine("2.\tLogin");
                Console.WriteLine("3.\tStatistics");
                Console.WriteLine("4.\tExit");
                
                Console.Write("Enter the number: ");
                if (!int.TryParse(Console.ReadLine(), out var num))
                {
                    Console.WriteLine("The only numbers can be entered. Try again");
                    continue;
                }
                
                switch (num)
                {
                    case 1:
                        await Register();
                        break;
                    case 2:
                        if (_currentSession.CountLoginFailed < 3)
                        {
                            await Login();
                        }
                        else
                        {
                            Console.WriteLine("You were temporarily blocked due to incorrect authorization!");
                        }

                        break;
                    case 3:
                        //todo statmenu
                        break;
                    case 4:
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Incorrect number. Try again");
                        break;
                }
                
                if (_stopwatch.ElapsedMilliseconds > 30000)
                {
                    _currentSession.CountLoginFailed = 0;
                    _stopwatch.Stop();
                }
            }
        }

        private async Task Register()
        {
            Console.WriteLine("Registration");

            var response = await GetResponse("api/auth/register");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonFromApi = await response.Content.ReadAsStringAsync();

                _currentSession = JsonSerializer.Deserialize<Session>(jsonFromApi);
                
                await new SessionMenu(_client, _currentSession).Start();
                return;
            }

            Console.WriteLine(response.StatusCode == HttpStatusCode.BadRequest
                ? "Invalid register values!"
                : "Account do not created!");
        }

        private async Task Login()
        {
            Console.WriteLine("Login");

            var response = await GetResponse("api/auth/login");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonFromApi = await response.Content.ReadAsStringAsync();

                _currentSession = JsonSerializer.Deserialize<Session>(jsonFromApi);
                
                await new SessionMenu(_client, _currentSession).Start();
                return;
            }
            
            _currentSession.CountLoginFailed++;

            Console.WriteLine(response.StatusCode == HttpStatusCode.BadRequest
                ? "Invalid login values!"
                : "Account do not found!");

            if (_currentSession.CountLoginFailed == 3)
            {
                _stopwatch.Start();
            }
        }

        private async Task<HttpResponseMessage> GetResponse(string url)
        {
            var userName = GetStringFromUser("Enter your user name:");
            var password = GetStringFromUser("Enter your password:");
                
            var uri = _client.BaseAddress + url;

            var user = new RequestUser
            {
                UserName = userName,
                Password = password
            };

            var userJson = JsonSerializer.Serialize(user);

            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            return await _client.PostAsync(uri, content);
        }

        private string GetStringFromUser(string message)
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
    }
}
