using System;
using Newtonsoft.Json;

namespace RSPGame.Models
{
    [Serializable]
    public class Session
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }
        
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("gamerInfo")]
        public GamerInfo GamerInfo { get; set; }
    }
}