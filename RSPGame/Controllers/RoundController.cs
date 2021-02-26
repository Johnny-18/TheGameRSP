using Microsoft.AspNetCore.Mvc;
using RSPGame.Storage;
using System.Linq;
using RSPGame.Models.GameModel;
using RSPGame.Services.Rsp;

namespace RSPGame.Controllers
{
    [ApiController]
    [Route("api/round")]
    public class RoundController : ControllerBase
    {
        private readonly RoundStorage _roundStorage;
        private readonly IRspService _rspService;

        public RoundController(RoundStorage roundStorage, IRspService rspService)
        {
            _roundStorage = roundStorage;
            _rspService = rspService;
        }

        [HttpPost("{roomId}/{userName}")]
        public IActionResult PostGameRound([FromBody] GameActions action, [FromRoute] string userName, [FromRoute] int roomId)
        {
            if (roomId < 1 || roomId > 1000)
                return BadRequest(roomId);

            var gamerReq = new GamerStep
            {
                UserName = userName,
                UserAction = action
            };

            _roundStorage.AddGamer(roomId, gamerReq);

            return Ok();
        }

        [HttpGet("{roomId}/{userName}")]
        public IActionResult GetGameRound([FromRoute] string userName, [FromRoute] int roomId)
        {
            if (roomId < 1 || roomId > 1000)
                return BadRequest(roomId);

            if (!_roundStorage.ContainRoom(roomId))
                return NotFound(roomId);

            var gamers = _roundStorage.PeekGamers(roomId).ToArray();

            if (gamers.Count() < 2)
                return NotFound(roomId);

            if (gamers.Count() > 2)
                return Conflict();

            var gamer1 = gamers.First();
            var gamer2 = gamers.Last();

            var result = _rspService.GetWinner(gamer1.UserAction, gamer2.UserAction);

            if (gamer1.UserName == userName)
                return Ok(result);

            return Ok(_rspService.InverseResult(result));
        }
    }
}
