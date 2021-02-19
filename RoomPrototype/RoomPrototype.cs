using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RoomPrototype
{
    /*комната, которая будет создаваться объявлением нового объекта текущего класса,
     удаляться(Dispose), а также проверять, что в комнате находится два человека(GameInfo).
    Как только в комнате нужное количество, вызывается метод Start. В методе старт находится таймер(Timer),
    а также вся логика проведения серии. У комнаты также есть свой ID, по которому пользователь может
    зайти в определенную комнату: такая комната называется приватной. Если комната не приватная, значит у
    комнаты айди равен 0 (его нет). Комната удаляется, если в ней находится 0 людей.
     */

    /*Чтобы хранить все комнаты, можно использовать коллекцию, из которой также придется удалть объект
     
    Как вариант, определять нужный нам элемент по ссылкам в памяти.
     */

    public class RoomPrototype : IDisposable
    {
        private readonly int _id = 0;

        private readonly List<GameInfo> _gamers;

        //private readonly object _obj = new object();

        public RoomPrototype()
        {
            _gamers = new List<GameInfo>();
            Task.Run(GamersCheck);
        }

        public RoomPrototype(int id)
        {
            _id = id;
            _gamers = new List<GameInfo>();
            Task.Run(GamersCheck);
        }

        private async void GamersCheck()
        {
            while (true)
            {
                if (_gamers.Count == 2)
                {
                    await StartGame();
                    Dispose();
                    break;
                }
            }
        }

        private Task StartGame()
        {
            Thread.Sleep(1000);
            Console.WriteLine($"Game in room #{_id} is starting.");
            return Task.CompletedTask;
        }

        public Task AddGamer(GameInfo gamer)
        {
            if (gamer == null) throw new ArgumentNullException();

            _gamers.Add(gamer);
            return Task.CompletedTask;

            //lock (_obj)
            //{
            //    _gamers.Add(gamer);
            //}
        }

        public void Dispose()
        {
            //удаление из коллекции, например
        }
    }
}
