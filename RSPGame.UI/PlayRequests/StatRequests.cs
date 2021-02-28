using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.UI.Models;

namespace RSPGame.UI.PlayRequests
{
    public static class StatRequests
    {
        public static void GetGeneralStat(HttpClient client)
        {
            Console.WriteLine("General statistics");
            
            var requestOptions = new RequestOptions
            {   
                Method = RequestMethod.Get,
                Address = client.BaseAddress + "api/stat/general"
            };

            var response = RequestHandler.HandleRequest(client, requestOptions);
            if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                var gamerInfos = JsonConvert.DeserializeObject<List<GamerInfo>>(response.Content);
                foreach (var gamerInfo in gamerInfos)
                {
                    Console.WriteLine(gamerInfo.ToString());
                }
                
                return;
            }

            Console.WriteLine("Not enough information for general statistics!");
        }
        
        public static void SaveOnlineTime(HttpClient client, Session session)
        {
            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/stat/individual/{session.UserName}",
                Method = RequestMethod.Post,
                Body = JsonConvert.SerializeObject(session.GamerInfo.OnlineTime)
            };
            
            RequestHandler.HandleRequest(client, requestOptions);
        }
    }
}