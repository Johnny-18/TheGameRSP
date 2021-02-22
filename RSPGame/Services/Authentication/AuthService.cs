using System;
using System.Collections.Concurrent;
using RSPGame.Models;
using RSPGame.Storage;

namespace RSPGame.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly RspRepository _repository;

        private readonly IJwtAuthenticationManager _authenticationManager;

        private readonly PasswordHashGenerator _hashGenerator;

        public AuthService(IJwtAuthenticationManager manager, RspRepository repository, PasswordHashGenerator hashGenerator)
        {
            _hashGenerator = hashGenerator;
            _authenticationManager = manager;
            _repository = repository;
        }
        
        public Session Register(RequestUser userForRegister)
         {
            if (userForRegister == null)
                throw new ArgumentNullException(nameof(userForRegister));

            var id = Guid.NewGuid();

            //generate password hash
            var passwordHash = _hashGenerator.GenerateHash(userForRegister.Password);
            
            //create user
            var user = new User
            {
                Id = id,
                UserName = userForRegister.UserName,
                GamerInfo = new GamerInfo(id),
                PasswordHash = passwordHash
            };

            if (_repository.Users == null)
                _repository.Users = new ConcurrentDictionary<string, User>();
            
            //try to add new user
            if (!_repository.Users.TryAdd(user.UserName, user))
                return null;
            
            //get token for new user
            var token = _authenticationManager.Authenticate(user.UserName, user.PasswordHash);
                
            //return new session with token
            return new Session
            {
                Id = user.Id,
                GamerInfo = user.GamerInfo,
                Token = token
            };
        }

        public Session Login(RequestUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!_repository.Users.TryGetValue(user.UserName, out var userFromStorage))
                return null;

            if (!_hashGenerator.AreEqual(user.Password, userFromStorage.PasswordHash))
                return null;

            //get user token
            var token = _authenticationManager.Authenticate(userFromStorage.UserName, userFromStorage.PasswordHash);
            
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