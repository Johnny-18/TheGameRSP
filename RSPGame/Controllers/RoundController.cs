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
        [HttpPost("{roomId}/{roundId}")]
        public IActionResult PostGameRound([FromBody] GameActions action, [FromRoute] int roomId, [FromRoute] int roundId)
        {
            if (roomId < 1 || roomId > 1000)
                return BadRequest(roomId);

            if (_roundStorage.DictionaryRound.ContainsKey(roomId))
            {
                var result = _rspService.GetWinner(_roundStorage.DictionaryRound[roomId], action);
                return Ok(result);
            }

            if (!_roundStorage.DictionaryRound.TryAdd(roomId, action))
                return Conflict(roomId);

            return Ok();
        }
    }
}
