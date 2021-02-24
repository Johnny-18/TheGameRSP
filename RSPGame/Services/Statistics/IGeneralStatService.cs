using System.Collections.Generic;
using System.Threading.Tasks;
using RSPGame.Models;

namespace RSPGame.Services.Statistics
{
    public interface IGeneralStatService
    {
        Task<IEnumerable<GamerInfo>> GetStat();
    }
}