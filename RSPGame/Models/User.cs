using System;
using System.Text.Json.Serialization;

namespace RSPGame.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        
        [JsonPropertyName("passwordHash")]
        public string PasswordHash { get; set; }

        [JsonPropertyName("gamerInfo")]
        public GamerInfo GamerInfo { get; set; }
    }
}