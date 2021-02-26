using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.Services;
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
            // var session = new Session
            // {
            //     UserName = "dsfds",
            //     Token = "wjdyweqtfdyguhwqijokdowjqhgfdqrwtfyudhijokqwdkjmqwhngydtfrqwtydguhiqow12",
            //     GamerInfo = new GamerInfo
            //     {
            //         UserName = "dfsf",
            //         OnlineTime = new TimeSpan(1, 1, 1, 1)
            //     }
            // };
            // var json = JsonConvert.SerializeObject(session);
            //
            // var ses = JsonConvert.DeserializeObject<Session>(json);
            
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
                Console.WriteLine("ERROR:\tCheck your internet connection and run game again!");
            }
        }
    }
}