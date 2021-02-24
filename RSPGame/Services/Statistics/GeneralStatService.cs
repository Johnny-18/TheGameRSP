using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RSPGame.Models;
using RSPGame.Storage;

namespace RSPGame.Services.Statistics
{
    public class GeneralStatService : IGeneralStatService
    {
        private readonly RspStorage _storage;

        public GeneralStatService(RspStorage storage)
        {
            _storage = storage;
        }
        
        public async Task<IEnumerable<GamerInfo>> GetStat()
        {
            var users = (await _storage.GetUsers()).Where(x => x.GamerInfo.Games > 10).Take(10).ToList();
            var result = new List<GamerInfo>();
            
            foreach (var user in users)
            {
                if (user.GamerInfo != null)
                {
                    result.Add(user.GamerInfo);
                }
            }

            return result;
        }
    }
}