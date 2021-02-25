using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RSPGame.UI
{
    class GameLogic
    {
        public void StartGame(HttpClient client, string[] usersName, int roomId)
        {
            Console.Clear();
            Console.WriteLine($"\nRoom ID: {roomId}\n");
            Console.WriteLine($"Match: {string.Join(" vs ", usersName)}");
        }
    }
}
