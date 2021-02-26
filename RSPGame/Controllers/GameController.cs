using Microsoft.AspNetCore.Mvc;
using RSPGame.Storage;
using System.Linq;
using RSPGame.Models;
using RSPGame.Services;

namespace RSPGame.Controllers
{
    [ApiController]
    [Route("api/game")]
    public class GameController : ControllerBase
    {
        private readonly GameStorage _gameStorage;

        private readonly SinglePlayerService _bot;

        public GameController(GameStorage gameStorage, SinglePlayerService bot)
        {
            _gameStorage = gameStorage;
            _bot = bot;
        }

        [HttpPost("{roomId}")]
        public IActionResult PostGame([FromBody] GamerInfo[] gamers, [FromRoute] int roomId)
        {
            if (roomId == 0 || gamers == null)
                return BadRequest();

            if (_gameStorage.DictionaryGame.TryAdd(roomId, gamers))
                return Ok();
            
            return Conflict();
        }

        [HttpGet("{roomId}")]
        public IActionResult GetGame([FromRoute] int roomId)
        {
            if (roomId < 0)
                return BadRequest(roomId);
            
            //check key
            if (!_gameStorage.DictionaryGame.ContainsKey(roomId))
                return NotFound();

            //get gamers from storage
            if(_gameStorage.DictionaryGame.TryGetValue(roomId, out var gamers))
                return Ok(gamers);

            return NoContent();
        }

        [HttpGet("bot")]
        public IActionResult GameWithBot([FromBody] GameActions gamerAction)
        {
            if (gamerAction == GameActions.None)
                return BadRequest();

            var result = _bot.PlayWithBot(gamerAction);

            return Ok(result);
        }
    }
}
