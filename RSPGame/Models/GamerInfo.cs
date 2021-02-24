using System;
using System.Text.Json.Serialization;

namespace RSPGame.Models
{
    public class GamerInfo
    {
        public GamerInfo()
        {
        }
        
        public GamerInfo(Guid id)
        {
            UserId = id;
        }
        
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        
        [JsonPropertyName("countScissors")]
        public int CountScissors { get; set; }
        
        [JsonPropertyName("countPapers")]
        public int CountPapers { get; set; }
        
        [JsonPropertyName("countRocks")]
        public int CountRocks { get; set; }
        
        [JsonPropertyName("countWins")]
        public int CountWins { get; set; }
        
        [JsonPropertyName("countLoses")]
        public int CountLoses { get; set; }
        
        [JsonPropertyName("countDraws")]
        public int CountDraws { get; set; }
        
        [JsonPropertyName("onlineTime")]
        public TimeSpan OnlineTime { get; set; }

        [JsonIgnore]
        public int Games
        {
            get
            {
                return CountLoses + CountDraws + CountWins;
            }
        }
    }
}