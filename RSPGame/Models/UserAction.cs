namespace RSPGame.Models
{
    public class UserAction
    {
        public UserAction(string userName, GameActions actions)
        {
            UserName = userName;
            Actions = actions;
        }

        public string UserName { get; }
        public GameActions Actions { get; }
    }
}
