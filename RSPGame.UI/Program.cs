using System;
using System.Net.Http;
using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.Services.FileWorker;
using RSPGame.UI.Menus;

namespace RSPGame.UI
{
    public static class Program
    {
        private static HttpClient _client;

        private static Session _session;
        
        private static string _path = "baseUrl.json";

        public static async Task Main()
        {
            var fileWorker = new FileWorker();
            var baseAddress = await fileWorker.DeserializeAsync<BaseAddress>(_path);
            if (baseAddress == null || string.IsNullOrEmpty(baseAddress.BaseUrl))
            {
                Console.WriteLine("Something going wrong with file!");
                return;
            }

            _client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress.BaseUrl)
            };

            _session = new Session();

            try
            {
                await new AuthorizationMenu(_client, _session).Start();
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("\n\nERROR:\tCheck your internet connection and run game again!\n");
            }
        }
    }
}