using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
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

        public async Task<ConcurrentDictionary<string, User>> GetUsers()
        {
            if (_users == null)
            {
                await GetFromFile(_path);
            }

            return _users;
        }

        private async Task GetFromFile(string path)
        {
            _users = await _fileWorker.DeserializeAsync<ConcurrentDictionary<string, User>>(path);

            if (_users == null)
                _users = new ConcurrentDictionary<string, User>();
        } 
    }
}