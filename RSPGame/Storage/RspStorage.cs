using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RSPGame.Models;
using RSPGame.Models.OptionsModel;
using RSPGame.Services;

namespace RSPGame.Storage
{
    public class RspStorage
    {
        public RspStorage(IFileWorker fileWorker, IOptions<FilesOptions> path)
        {
            _fileWorker = fileWorker;
            _path = path.Value.Users;
        }

        private readonly IFileWorker _fileWorker;

        private readonly string _path;

        private ConcurrentDictionary<string, User> _users;

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            await CheckCollection();

            _users.TryGetValue(userName, out var user);

            return user;
        }

        public async Task<bool> TryAddUserAsync(User user)
        {
            await CheckCollection();
            
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (_users.ContainsKey(user.UserName))
            {
                return false;
            }

            if (!_users.TryAdd(user.UserName, user)) 
                return false;

            await SaveToFile();
            return true;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            await CheckCollection();
            
            return _users as IEnumerable<User>;
        }

        private async Task CheckCollection()
        {
            if (_users == null)
            {
                await GetFromFile(_path);
            }
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