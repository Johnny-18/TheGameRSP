namespace RSPGame.Models
{
    public class GameResponse
    {
        private readonly string _opponentName;
        private readonly int _idRoom;

        public GameResponse(string opponentName, int idRoom)
        {
            _opponentName = opponentName;
            _idRoom = idRoom;
        }

        public string GetOpponentName => _opponentName;

        public int GetIdRoom => _idRoom;

    }
}
