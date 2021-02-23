using System;
using System.Threading;
using System.Threading.Tasks;

namespace Prototype
{
    class Program
    {
        static async Task Main()
        {        
            var room = new RoomPrototype();

            await room.AddGamer(new GameInfo());
            //Task.Run(() => room.AddGamer(new GameInfo()));

           Console.ReadKey();
        }
    }
}
