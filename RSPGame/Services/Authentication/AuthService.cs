using System;
using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.Storage;

namespace RSPGame.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly RspRepository _repository;

        private readonly IJwtTokenGenerator _tokenGenerator;

        private readonly PasswordHashGenerator _hashGenerator;

        public AuthService(IJwtTokenGenerator manager, RspRepository repository, PasswordHashGenerator hashGenerator)
        {
            _hashGenerator = hashGenerator;
            _tokenGenerator = manager;
            _repository = repository;
        }
        
        public async Task<Session> Register(RequestUser userForRegister)
         {
            if (userForRegister == null)
                throw new ArgumentNullException(nameof(userForRegister));

            var id = Guid.NewGuid();

            //generate password hash
            var passwordHash = await _hashGenerator.GenerateHash(userForRegister.Password);
            
            //create user
            var user = new User
            {
                Id = id,
                UserName = userForRegister.UserName,
                GamerInfo = new GamerInfo(id),
                PasswordHash = passwordHash
            };

            //try to add new user
            if (!await _repository.TryAddUser(user))
                return null;

            //get token for new user
            var token = _tokenGenerator.GenerateToken(user.UserName, user.PasswordHash);
                
            //return new session with token
            return new Session
            {
                Id = user.Id,
                GamerInfo = user.GamerInfo,
                Token = token
            };
        }

        public async Task<Session> Login(RequestUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var userFromStorage = await _repository.GetUserByUserName(user.UserName);
            if (userFromStorage == null)
                return null;

            if (!await _hashGenerator.AreEqual(user.Password, userFromStorage.PasswordHash))
                return null;

            //get user token
            var token = _tokenGenerator.GenerateToken(userFromStorage.UserName, userFromStorage.PasswordHash);
            
            //create session
            return new Session
            {
                Id = userFromStorage.Id,
                GamerInfo = userFromStorage.GamerInfo,
                Token = token
            };
        }

        public void Logout(Session user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            
            //clear token
            user.Token = null;
        }
    }
}