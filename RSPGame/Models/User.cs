using System.Text.Json.Serialization;
using RSPGame.Models.GameModel;

namespace RSPGame.Models
{
    public class User
    {

        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        
        [JsonPropertyName("passwordHash")]
        public string PasswordHash { get; set; }

        [JsonPropertyName("gamerInfo")]
        public GamerInfo GamerInfo { get; set; }
    }
}