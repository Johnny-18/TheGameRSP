using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Prototype;

namespace RSPGame.Storages
{
    public class SearchingPlayerStorage
    {
        //todo: присоединиться по номеру комнаты
        //todo: логика игры
        //todo: сбор статистики 
        //todo: отправление ходов на сервер
        //todo: отправление результатов раунда игрокам
        //todo: отправление результатов матча игрокам

        private static readonly ConcurrentQueue<GameInfo> ListGameInfos
            = new ConcurrentQueue<GameInfo>();
        private static readonly ConcurrentQueue<RoomPrototype> ListRooms
            = new ConcurrentQueue<RoomPrototype>();

        public SearchingPlayerStorage()
        {
            Task.Run(CheckQueue);
        }

        private async void CheckQueue()
        {
            while (true)
            {
                if (ListGameInfos.IsEmpty) continue;
                if (!ListRooms.IsEmpty)
                {
                    ListRooms.TryDequeue(out RoomPrototype room);
                    ListGameInfos.TryDequeue(out GameInfo gamer);
                    if (room == null)
                        throw new ArgumentNullException(nameof(room));
                    await room.AddGamer(gamer);
                }
                else await CreateRoom();
            }
        }

        private async Task CreateRoom()
        {
            var room = new RoomPrototype();
            ListGameInfos.TryDequeue(out GameInfo gamer);

            //Console.WriteLine("room created!");

            await room.AddGamer(gamer);

            ListRooms.Enqueue(room);
        }

        public Task AddToQueue(GameInfo gamer)
        {
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            ListGameInfos.Enqueue(gamer);
            return Task.CompletedTask;
        }

    }
}
