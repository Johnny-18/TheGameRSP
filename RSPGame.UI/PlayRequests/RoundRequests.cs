using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using RSPGame.Models.Game;
using RSPGame.UI.Models;

namespace RSPGame.UI.PlayRequests
{
    public static class RoundRequests
    {
        public static void AddRoundToRoom(HttpClient client, Round round, string token, int roomId)
        {
            if (client == null || round == null) 
                return;

            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/rounds/{roomId}",
                Method = RequestMethod.Post,
                Body = JsonConvert.SerializeObject(round),
                Token = token
            };

            RequestHandler.HandleRequest(client, requestOptions);
        }

        public static bool Put(HttpClient client, string token, string url)
        {
            if (client == null) 
                return false;

            var requestOptions = new RequestOptions
            {
                Address = url,
                Method = RequestMethod.Put,
                Token = token
            };
            
            var response = RequestHandler.HandleRequest(client, requestOptions);
            if (response == null)
                return false;
                
            if(response.StatusCode == (int)HttpStatusCode.OK)
                return true;

            return false;
        }
    }
}