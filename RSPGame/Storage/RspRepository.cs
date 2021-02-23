using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.Services;

namespace RSPGame.Storage
{
    public class RspRepository
    {
        public RspRepository(IFileWorker fileWorker, string path)
        {
            _fileWorker = fileWorker;
            _path = path;
        }

        private readonly IFileWorker _fileWorker;

        private readonly string _path;

        private ConcurrentDictionary<string, User> _users;

        public async Task<User> GetUserByUserName(string userName)
        {
            if (_users == null)
            {
                await GetFromFile(_path);
            }

            _users.TryGetValue(userName, out var user);

            return user;
        }

        public async Task<bool> TryAddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!_users.TryAdd(user.UserName, user)) 
                return false;

            await SaveToFile();
            return true;

        }

        private async Task SaveToFile()
        {
            if (_users != null && _users.Count > 0)
            {
                await _fileWorker.SaveToFileAsync(_path, _users);
            }
        }

        private async Task GetFromFile(string path)
        {
            _users = await _fileWorker.DeserializeAsync<ConcurrentDictionary<string, User>>(path) ?? new ConcurrentDictionary<string, User>();
        } 
    }
}