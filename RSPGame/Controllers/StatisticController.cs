using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RSPGame.Models;
using RSPGame.Models.GameModel;
using RSPGame.Services.Statistics;
using RSPGame.Storage;

namespace RSPGame.Controllers
{
    [ApiController]
    [Route("api/stat")]
    public class StatisticController : ControllerBase
    {
        private readonly IGeneralStatService _generalStat;

        private readonly IIndividualStatService _individualStat;

        private readonly IRspStorage _storage;
        private readonly IRspStorage _rspStorage;

        public StatisticController(IIndividualStatService individualStat, IGeneralStatService generalStat, IRspStorage storage, IRspStorage rspStorage)
        {
            _individualStat = individualStat;
            _generalStat = generalStat;
            _storage = storage;
            _rspStorage = rspStorage;
        }

        [HttpGet("general")]
        public async Task<IActionResult> GetGeneralStat()
        {
            var result = await _generalStat.GetStatAsync(_storage);
            if (result == null || result.Any() == false)
                return NoContent();
            
            return Ok(result);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveToFile([FromBody] GamerInfo gamer)
        {
            var user = await _storage.GetUserByUserNameAsync(gamer.UserName);
            var newGamer = _individualStat.ChangeGamerInfoAfterRound(user.GamerInfo, gamer);

            var newUser = new User
            {
                UserName = user.UserName,
                PasswordHash = user.PasswordHash,
                GamerInfo = newGamer
            };

            _rspStorage.TryUpdate(newUser.UserName, newUser);

            await _rspStorage.SaveToFile();

            return Ok();
        }

        [HttpGet("individual/{userName}")]
        public async Task<IActionResult> GetIndividualStat([FromRoute] string userName)
        {
            var user = await _storage.GetUserByUserNameAsync(userName);

            return Ok(user.GamerInfo);
        }

        //todo change online time
        //todo change individual stat
        //todo get individual stat
    }
}