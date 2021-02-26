using System;
using System.Text;
using Newtonsoft.Json;
using RSPGame.Models.JsonConverter;

namespace RSPGame.Models
{
    [Serializable]
    public class GamerInfo
    {
        public GamerInfo()
        {
        }
        
        public GamerInfo(string userName)
        {
            UserName = userName;
        }
        
        [JsonProperty("userName")]
        public string UserName { get; set; }
        
        [JsonProperty("countScissors")]
        public int CountScissors { get; set; }
        
        [JsonProperty("countPapers")]
        public int CountPapers { get; set; }
        
        [JsonProperty("countRocks")]
        public int CountRocks { get; set; }
        
        [JsonProperty("countWins")]
        public int CountWins { get; set; }
        
        [JsonProperty("countLoses")]
        public int CountLoses { get; set; }
        
        [JsonProperty("countDraws")]
        public int CountDraws { get; set; }
        
        [JsonProperty("onlineTime")]
        [JsonConverter(typeof(TimespanConverter))]
        public TimeSpan OnlineTime { get; set; }

        [JsonIgnore]
        public int Games => CountLoses + CountDraws + CountWins;

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