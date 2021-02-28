using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.UI.Game;
using RSPGame.UI.PlayRequests;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RSPGame.UI.Menus
{
    public class PrivateRoomMenu
    {
        private readonly Session _currentSession;

        private readonly HttpClient _client;

        public PrivateRoomMenu(HttpClient client, Session currentSession)
        {
            _client = client;
            _currentSession = currentSession;
        }

        public Task Start()
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
                        CreateRoomAction();
                        break;
                    case 2:
                        JoinRoomAction();
                        break;
                    case 3:
                        return Task.CompletedTask;
                }
            }
        }

        private async void CreateRoomAction()
        {
            string json = null;
            try
            {
                json = await RoomRequests.PostAsync(_client, _currentSession.GamerInfo, "create");
            }
            catch (HttpRequestException)
            {
                PrintError();
                return;
            }
            catch (AggregateException)
            {
                PrintError();
                return;
            }
            catch (NullReferenceException)
            {
                PrintError();
                return;
            }

            if (json == null)
            {
                PrintError();
                return;
            }
            var id = JsonConvert.DeserializeObject<int>(json);

            Console.WriteLine($"\nRoom with id {id} has been created!");
            Console.WriteLine("\nWaiting for opponent\n");

            var result = (await GameRequests.GetGame(_client, id))?.ToArray();
            if (result == null) return;

            var opponent1 = result
                .FirstOrDefault(x => !x.Equals(_currentSession.GamerInfo.UserName));
            try
            {
                new GameLogic().StartGame(_client, _currentSession.GamerInfo.UserName, opponent1, id);
                await _client.DeleteAsync($"api/rooms/stop/{id}");
            }
            catch (HttpRequestException)
            {
                PrintError();
            }
            catch (AggregateException)
            {
                PrintError();
            }
        }

        private void PrintError()
        {
            Console.WriteLine("\nERROR:\tCheck your internet connection\n");
        }

        private async void JoinRoomAction()
        {
            Console.Write("Enter the id of the desired room: ");

            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("\nERROR:\tThe only numbers can be entered. Try again\n");
                return;
            }

            if (id < 1 || id > 1000)
            {
                Console.WriteLine("\nERROR:\tIncorrect number. Try again\n");
                return;
            }

            if (RoomRequests.JoinAsync(_client, _currentSession.GamerInfo, id) == null)
                return;

            var result = (await GameRequests.GetGame(_client, id))?.ToArray();
            if (result == null) return;

            var opponent2 = result
                .FirstOrDefault(x => !x.Equals(_currentSession.GamerInfo.UserName));

            try
            {
                new GameLogic().StartGame(_client, _currentSession.GamerInfo.UserName, opponent2, id);
            }
            catch (HttpRequestException)
            {
                PrintError();
            }
            catch (AggregateException)
            {
                PrintError();
            }
            catch (NullReferenceException)
            {
                PrintError();
            }
        }
    }
}