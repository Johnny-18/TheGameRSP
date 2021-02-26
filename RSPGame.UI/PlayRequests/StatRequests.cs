using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.UI.Models;

namespace RSPGame.UI.PlayRequests
{
    public static class StatRequests
    {
        public static void GetGeneralStat(HttpClient client)
        {
            Console.WriteLine("General statistics");

            var response = AuthRequests.GetResponse(client, "api/stat/general");
            if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                var gamerInfos = JsonSerializer.Deserialize<List<GamerInfo>>(response.Content);
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