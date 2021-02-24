using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using RSPGame.Models;

namespace RSPGame.UI.PlayRequests
{
    public static class RoomRequests
    {
        public static async void QuickSearch(HttpClient client, GamerInfo gamer)
        {
            var json = JsonSerializer.Serialize(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await client.PostAsync($"/api/rooms/find", content);
        }
    }
}
