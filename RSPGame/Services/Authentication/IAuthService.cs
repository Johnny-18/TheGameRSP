using RSPGame.Models;

namespace RSPGame.Services.Authentication
{
    public interface IAuthService
    {
        public Session Register(RequestUser userForRegister);
        
        public Session Login(RequestUser user);

        public void Logout(Session user);
    }
}