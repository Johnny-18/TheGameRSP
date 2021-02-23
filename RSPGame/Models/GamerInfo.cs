using System;

namespace RSPGame.Models
{
    public class GamerInfo
    {
        public GamerInfo(Guid id)
        {
            UserId = id;
        }
        
        public Guid UserId { get; set; }
        
        public int CountScissors { get; set; }
        
        public int CountPaper { get; set; }
        
        public int CountRock { get; set; }
        
        public int CountWin { get; set; }
        
        public int CountLose { get; set; }
        
        public int CountDraw { get; set; }
        
        public TimeSpan OnlineTime { get; set; }
    }
}