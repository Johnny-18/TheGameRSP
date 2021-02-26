using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RSPGame.Models;
using RSPGame.Services;
using RSPGame.Storage;

namespace RSPGame.Controllers
{
    [ApiController]
    [Route("api/round")]
    public class RoundController : ControllerBase
    {
        private readonly RoundStorage _roundStorage;
        private readonly RspService _rspService;

        public RoundController(RoundStorage roundStorage, RspService rspService)
        {
            _roundStorage = roundStorage;
            _rspService = rspService;
        }

        [HttpPost("{roomId}/{userName}")]
        public IActionResult PostGameRound([FromBody] GameActions action, [FromRoute] string userName, [FromRoute] int roomId)
        {
            if (roomId < 1 || roomId > 1000)
                return BadRequest(roomId);
            _roundStorage.DictionaryRound[roomId].Add(new UserAction(userName, action));

            return Ok();
        }

        [HttpPost("{roomId}/{userName}")]
        public IActionResult GetGameRound([FromRoute] string userName, [FromRoute] int roomId)
        {
            if (roomId < 1 || roomId > 1000)
                return BadRequest(roomId);

            var gamer1 = _roundStorage.DictionaryRound[roomId]
                .FirstOrDefault(x => x.UserName == userName);

            if (gamer1 == null)
                return NotFound();

            var gamer2 = _roundStorage.DictionaryRound[roomId]
                .FirstOrDefault(x => x.UserName != userName);

            if (gamer2 == null)
                return NotFound();

            return Ok(_rspService.GetWinner(gamer1.Actions, gamer2.Actions));

        }
    }
}
