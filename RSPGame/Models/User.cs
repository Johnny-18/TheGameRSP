using System;
using Newtonsoft.Json;

namespace RSPGame.Models
{
    [Serializable]
    public class User
    {

        [JsonProperty("userName")]
        public string UserName { get; set; }
        
        [JsonProperty("passwordHash")]
        public string PasswordHash { get; set; }

        [JsonProperty("gamerInfo")]
        public GamerInfo GamerInfo { get; set; }
    }
}