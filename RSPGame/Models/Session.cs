using System;

namespace RSPGame.Models
{
    public class Session
    {
        public Guid Id { get; set; }
        
        public string Token { get; set; }
        
        public GamerInfo GamerInfo { get; set; }
    }
}