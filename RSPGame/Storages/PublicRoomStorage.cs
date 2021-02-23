using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Prototype;

namespace RSPGame.Storages
{
    public class PublicRoomStorage
    {
        //todo: game logic
        //todo: statistics 
        //todo: post step on server
        //todo: get round result by gamers 
        //todo: get match result by gamers 

        private static readonly ConcurrentQueue<GameInfo> QueueGameInfos
            = new ConcurrentQueue<GameInfo>();
        private static readonly ConcurrentQueue<RoomPrototype> QueueRooms
            = new ConcurrentQueue<RoomPrototype>();

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
                    QueueRooms.TryDequeue(out RoomPrototype room);
                    QueueGameInfos.TryDequeue(out GameInfo gamer);
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
            QueueGameInfos.TryDequeue(out GameInfo gamer);

            await room.AddGamer(gamer);

            QueueRooms.Enqueue(room);
        }

        public Task AddToQueue(GameInfo gamer)
        {
            if (gamer == null)
                throw new ArgumentNullException(nameof(gamer));

            QueueGameInfos.Enqueue(gamer);
            return Task.CompletedTask;
        }

    }
}
