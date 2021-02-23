using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using RSPGame.Models;

namespace RSPGame.Storage
{
    public class PublicRoomStorage
    {
        //todo: game logic
        //todo: statistics 
        //todo: post step on server
        //todo: get round result by gamers 
        //todo: get match result by gamers 

        private static readonly ConcurrentQueue<GamerInfo> QueueGameInfos
            = new ConcurrentQueue<GamerInfo>();
        private static readonly ConcurrentQueue<Room> QueueRooms
            = new ConcurrentQueue<Room>();

        public PublicRoomStorage()
        {
            Task.Run(CheckQueue);
        }

        private async void CheckQueue()
        {
            while (true)
            {
                if (QueueGameInfos.IsEmpty) continue;
                if (!QueueRooms.IsEmpty)
                {
                    QueueRooms.TryDequeue(out Room room);
                    QueueGameInfos.TryDequeue(out GamerInfo gamer);
                    if (room == null)
                        throw new ArgumentNullException(nameof(room));
                    await room.AddGamer(gamer);
                }
                else await CreateRoom();
            }
        }

        private async Task CreateRoom()
        {
            var room = new Room();
            QueueGameInfos.TryDequeue(out GamerInfo gamer);

            await room.AddGamer(gamer);

            QueueRooms.Enqueue(room);
        }

        public Task AddToQueue(GamerInfo gamer)
        {
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            QueueGameInfos.Enqueue(gamer);
            return Task.CompletedTask;
        }

    }
}
