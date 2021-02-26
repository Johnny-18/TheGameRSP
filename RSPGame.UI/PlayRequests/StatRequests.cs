using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RSPGame.Models.GameModel;

namespace RSPGame.UI.PlayRequests
{
    public static class StatRequests
    {
        public static async Task GetGeneralStat(HttpClient client)
        {
            Console.WriteLine("General statistics");

            var response = await AuthRequests.GetResponse(client, "api/stat/general");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonFromApi = await response.Content.ReadAsStringAsync();
                
                var gamerInfos = JsonSerializer.Deserialize<List<GamerInfo>>(jsonFromApi);
                foreach (var gamerInfo in gamerInfos)
                {
                    Console.WriteLine(gamerInfo.ToString());
                }
                
                return;
            }

            Console.WriteLine("Not enough information for general statistics!");
        }
    }
}