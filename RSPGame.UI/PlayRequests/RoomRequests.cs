﻿using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RSPGame.Models;

namespace RSPGame.UI.PlayRequests
{
    public static class RoomRequests
    {
        public static async Task<string> QuickSearch(HttpClient client, GamerInfo gamer)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var json = JsonSerializer.Serialize(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string result;

            try
            {
                var task = Task.Run(() => client.PostAsync($"/api/rooms/find", content));
                task.Wait();
                result = await task.Result.Content.ReadAsStringAsync();
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }

            return result;
        }

        public static async Task<string> CreateRoom(HttpClient client, GamerInfo gamer)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var json = JsonSerializer.Serialize(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string result;

            try
            {
                var task = Task.Run(() => client.PostAsync($"/api/rooms/create", content));
                task.Wait();
                result = await task.Result.Content.ReadAsStringAsync();
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return null;
            }

            return result;
        }

        public static Task JoinRoom(HttpClient client, GamerInfo gamer, int id)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            var json = JsonSerializer.Serialize(gamer);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task;

            try
            {
                task = Task.Run(() => client.PostAsync($"/api/rooms/join?id={id}", content));
                task.Wait();
            }
            catch (AggregateException)
            {
                Console.WriteLine("\nERROR:\tCheck your internet connection\n\n");
                return Task.CompletedTask;
            }

            if (task.Result.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("\nThe room was not found. Check the number again.\n\n");
                return Task.CompletedTask;
            }

            Console.WriteLine("\nDone!\n\n");
            return Task.CompletedTask;
        }
    }
}
