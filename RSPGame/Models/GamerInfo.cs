using System;
using System.Text;
using System.Text.Json.Serialization;

namespace RSPGame.Models
{
    public class GamerInfo
    {
        public GamerInfo()
        {
        }
        
        public GamerInfo(string userName)
        {
            UserName = userName;
        }
        
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        
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

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"\nUser: {UserName}\n");
            stringBuilder.Append($"Games: {Games}\n");
            stringBuilder.Append($"Wins: {CountWins}, loses: {CountLoses}, draws: {CountDraws}\n");
            stringBuilder.Append($"Scissors: {CountScissors}, papers: {CountPapers}, rocks: {CountRocks}\n");
            
            stringBuilder.Append($"Online time: ");
            if (OnlineTime.Days != 0)
            {
                stringBuilder.Append(OnlineTime.Days.ToString());
            }

            if (OnlineTime.Hours != 0)
            {
                stringBuilder.Append(OnlineTime.Hours.ToString());
            }

            if (OnlineTime.Minutes != 0)
            {
                stringBuilder.Append(OnlineTime.Minutes.ToString());
            }
            
            stringBuilder.AppendLine();
            
            return stringBuilder.ToString();
        }
    }
}