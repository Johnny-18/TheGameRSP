using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static async void Start(HttpClient client, GamerInfo gamer)
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
                        var json = await RoomRequests.QuickSearch(client, gamer);
                        if (json == null) break;
                        var id = JsonConvert.DeserializeObject<int>(json);

                        var stopwatch = new Stopwatch();
                        Queue<Task> tasks = new Queue<Task>();
                        stopwatch.Start();

                        while (true)
                        {
                            Thread.Sleep(1500);
                            tasks.Enqueue(Task.Run(() => GameRequests.GetGame(client, id)));
                            if (stopwatch.ElapsedMilliseconds < 30000) continue;
                            Console.WriteLine("\nThe game could not be found. Please try again.\n\n");
                            stopwatch.Stop();
                            break;
                        }

                        await Task.WhenAll(tasks);

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
