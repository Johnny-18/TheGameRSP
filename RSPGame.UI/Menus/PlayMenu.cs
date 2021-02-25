using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.UI.PlayRequests;

namespace RSPGame.UI.Menus
{
    public static class PlayMenu
    {
        public static void Start(HttpClient client, GamerInfo gamer)
        {
            while (true)
            {
                int num;
                Console.WriteLine("1.\tQuick search");
                Console.WriteLine("2.\tPrivate room");
                Console.WriteLine("3.\tWith bot");
                Console.WriteLine("4.\tBack");

                while (true)
                {
                    Console.Write("Enter the number: ");
                    if (!int.TryParse(Console.ReadLine(), out num)) Console.WriteLine("The only numbers can be entered. Try again");
                    else if (num < 1 || num > 4) Console.WriteLine("Incorrect number. Try again");
                    else break;
                }
                Console.WriteLine();
                switch (num)
                {
                    case 1:
                        var json = RoomRequests.QuickSearch(client, gamer).Result;
                        if (json == null) break;
                        var id = JsonConvert.DeserializeObject<int>(json);

                        var counter = 0;
                        HttpResponseMessage response;
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();

                        while (true)
                        {
                            if (stopwatch.ElapsedMilliseconds < 2500) continue;
                            response = GameRequests.GetGame(client, id);
                            if (response.StatusCode == HttpStatusCode.OK) break;

                            counter++;
                            stopwatch.Restart();
                            if (counter == 12) break;
                        }

                        if (response.StatusCode == HttpStatusCode.NotFound)
                        { 
                            Console.WriteLine("\nThe game could not be found. Please try again.\n\n");
                            break;
                        }

                        json = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<string[]>(json);

                        Console.WriteLine("\nDone!\n\n");

                        break;
                    case 2:
                        PrivateRoomMenu.Start(client, gamer);
                        break;
                    case 3:
                        //
                        break;
                    case 4:
                        return;
                }
            }
        }
    }
}
