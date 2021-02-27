using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using RSPGame.Models.Game;
using RSPGame.UI.Models;

namespace RSPGame.UI.PlayRequests
{
    public static class RoundRequests
    {
        public static void AddRoundToRoom(HttpClient client, Round round, int roomId)
        {
            if (client == null || round == null) 
                return;

            var requestOptions = new RequestOptions
            {
                Address = client.BaseAddress + $"api/rounds/{roomId}",
                Method = RequestMethod.Post,
                Body = JsonConvert.SerializeObject(round)
            };

            var response = RequestHandler.HandleRequest(client, requestOptions);
        }
    }
}