using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.UI.PlayRequests;

namespace RSPGame.UI.Menus
{
    public static class PrivateRoomMenu
    {
        public static void Start(HttpClient client, GamerInfo gamer)
        {
            while (true)
            {
                int num;
                Console.WriteLine("1.\tCreate room");
                Console.WriteLine("2.\tJoin room");
                Console.WriteLine("3.\tBack");

                while (true)
                {
                    Console.Write("Enter the number: ");
                    if (!int.TryParse(Console.ReadLine(), out num)) Console.WriteLine("The only numbers can be entered. Try again");
                    else if (num < 1 || num > 3) Console.WriteLine("Incorrect number. Try again");
                    else break;
                }
                Console.WriteLine();
                switch (num)
                {
                    case 1:
                        var json = RoomRequests.CreateRoom(client, gamer).Result;
                        if (json == null) break;
                        var id = JsonConvert.DeserializeObject<int>(json);

                        Console.WriteLine($"\nRoom with id {id} has been created!");
                        Console.WriteLine("\nWaiting for opponent\n\n");

                        var result = GameRequests.GetGame(client, id)?.ToArray();

                        break;

                    case 2:
                        Console.Write("Enter the id of the desired room: ");

                        if (!int.TryParse(Console.ReadLine(), out var i))
                        {
                            Console.WriteLine("\nERROR:\tThe only numbers can be entered. Try again\n\n");
                            break;
                        }
                        else if (i < 1 || i > 1000)
                        {
                            Console.WriteLine("\nERROR:\tIncorrect number. Try again\n\n");
                            break;
                        }

                        RoomRequests.JoinRoom(client, gamer, i);
                        break;

                    case 3:
                        return;
                }
            }
        }
    }
}
