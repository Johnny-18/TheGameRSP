using System;
using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.Storage;

namespace RSPGame.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IRspStorage _storage;

        private readonly IJwtTokenGenerator _tokenGenerator;

        private readonly PasswordHashGenerator _hashGenerator;

        public AuthService(IJwtTokenGenerator manager, IRspStorage storage, PasswordHashGenerator hashGenerator)
        {
            _hashGenerator = hashGenerator;
            _tokenGenerator = manager;
            _storage = storage;
        }
        
        public async Task<Session> RegisterAsync(RequestUser userForRegister)
         {
            if (userForRegister == null)
                throw new ArgumentNullException(nameof(userForRegister));

            //generate password hash
            var passwordHash = _hashGenerator.GenerateHash(userForRegister.Password);
            
            //create user
            var user = new User
            {
                UserName = userForRegister.UserName,
                GamerInfo = new GamerInfo(userForRegister.UserName),
                PasswordHash = passwordHash
            };

            //try to add new user
            if (!await _storage.TryAddUserAsync(user))
                return null;

            //get token for new user
            var token = _tokenGenerator.GenerateToken(user.UserName, user.PasswordHash);
                
            //return new session with token
            return new Session
            {
                UserName = user.UserName,
                GamerInfo = user.GamerInfo,
                Token = token
            };
        }

        public async Task<Session> LoginAsync(RequestUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var userFromStorage = await _storage.GetUserByUserNameAsync(user.UserName);
            if (userFromStorage == null)
                return null;

            if (!_hashGenerator.AreEqual(user.Password, userFromStorage.PasswordHash))
                return null;

            //get user token
            var token = _tokenGenerator.GenerateToken(userFromStorage.UserName, userFromStorage.PasswordHash);
            
            //create session
            return new Session
            {
                UserName = userFromStorage.UserName,
                GamerInfo = userFromStorage.GamerInfo,
                Token = token
            };
        }
    }
}