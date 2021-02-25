using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RSPGame.Services;
using RSPGame.Services.Statistics;

namespace RSPGame.Controllers
{
    [ApiController]
    [Route("api/stat")]
    public class StatisticController : ControllerBase
    {
        private readonly IGeneralStatService _generalStat;

        private readonly IIndividualStatService _individualStat;

        private readonly IRspStorage _storage;

        public StatisticController(IIndividualStatService individualStat, IGeneralStatService generalStat, IRspStorage storage)
        {
            _individualStat = individualStat;
            _generalStat = generalStat;
            _storage = storage;
        }

        [HttpGet("general")]
        public async Task<IActionResult> GetGeneralStat()
        {
            var result = await _generalStat.GetStatAsync(_storage);
            if (result == null || result.Any() == false)
                return NoContent();
            
            return Ok(result);
        }

        //todo change online time
        //todo change individual stat
        //todo get individual stat
    }
}