using System;
using System.Net.Http;
using RSPGame.Models;
using RSPGame.UI.Menus;

namespace RSPGame.UI
{
    public static class Program
    {
        private static HttpClient _client;
        private static GamerInfo _gamer;

        static void Main()
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:5000")
            };
            _gamer = new GamerInfo();

            try
            {
                AuthorizationMenu.Start(_client, _gamer);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("ERROR:\tCheck your internet connection");
            }
            
        }
    }
}