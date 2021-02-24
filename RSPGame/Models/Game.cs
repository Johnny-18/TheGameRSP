namespace RSPGame.Models
{
    public class Game
    {
        public readonly string[] UsersName;
        public readonly int RoomId;

        public Game(string[] usersName, int roomId)
        {
            UsersName = usersName;
            RoomId = roomId;
        }

    }
}
