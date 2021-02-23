using System.Threading.Tasks;
using RSPGame.Models;

namespace RSPGame.Services.Authentication
{
    public interface IAuthService
    {
        public Task<Session> Register(RequestUser userForRegister);
        
        public Task<Session> Login(RequestUser user);

        public void Logout(Session user);
    }
}