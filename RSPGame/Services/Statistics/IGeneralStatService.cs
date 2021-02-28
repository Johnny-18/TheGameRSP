using System.Collections.Generic;
using System.Threading.Tasks;
using RSPGame.Models.GameModel;
using RSPGame.Storage;

namespace RSPGame.Services.Statistics
{
    public interface IGeneralStatService
    {
        Task<IEnumerable<GamerInfo>> GetStatAsync(IRspStorage storage);
    }
}