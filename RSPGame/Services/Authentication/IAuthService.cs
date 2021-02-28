using System.Threading.Tasks;
using RSPGame.Models;

namespace RSPGame.Services.Authentication
{
    public interface IAuthService
    {
        public Task<Session> RegisterAsync(RequestUser userForRegister);
        
        public Task<Session> LoginAsync(RequestUser user);
    }
}