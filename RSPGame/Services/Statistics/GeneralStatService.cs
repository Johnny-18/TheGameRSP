using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RSPGame.Models.GameModel;
using RSPGame.Storage;

namespace RSPGame.Services.Statistics
{
    public class GeneralStatService : IGeneralStatService
    {
        public async Task<IEnumerable<GamerInfo>> GetStatAsync(IRspStorage storage)
        {
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));
            
            var usersFromStorage = await storage.GetUsersAsync();
            if (usersFromStorage == null)
                return null;

            var users = usersFromStorage.Where(x => x.GamerInfo.Games > 10).OrderByDescending(x => x.GamerInfo.Games).Take(10).ToList();
            
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