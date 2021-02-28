using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.Models.GameModel;

namespace RSPGame.UI.PlayRequests
{
    public static class StatRequests
    {
        public static async Task GetGeneralStat(HttpClient client)
        {
            Console.WriteLine("General statistics");

            var response = await client.GetAsync("api/stat/general");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonFromApi = await response.Content.ReadAsStringAsync();
                
                var gamerInfos = JsonConvert.DeserializeObject<IEnumerable<GamerInfo>>(jsonFromApi);
                foreach (var gamerInfo in gamerInfos)
                {
                    Console.WriteLine(gamerInfo.ToString());
                }
                
                return;
            }

            Console.WriteLine("Not enough information for general statistics!");
        }

        public static void GetIndividualStat(HttpClient client, Session session)
        {
            Console.WriteLine("Individual statistics");

            var response = client.GetAsync($"api/stat/individual/{session.UserName}").Result;

            if (response == null)
                return;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = response.Content.ReadAsStringAsync().Result;

                var gamerInfos = JsonConvert.DeserializeObject<GamerInfo>(json);

                Console.WriteLine(gamerInfos.ToString());
            }

            Console.WriteLine("Something going wrong!");
        }
    }
}