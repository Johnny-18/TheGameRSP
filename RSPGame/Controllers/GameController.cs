using Microsoft.AspNetCore.Mvc;
using RSPGame.Storage;
using System.Linq;
using RSPGame.Models.GameModel;
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
        public IActionResult PostGame([FromBody] string[] usersName, [FromRoute] int roomId)
        {
            if (roomId == 0 || usersName.Any(string.IsNullOrWhiteSpace))
                return BadRequest();

            if (_gameStorage.DictionaryGame.TryAdd(roomId, usersName)) 
                return Ok();
            
            return Conflict();
        }

        [HttpGet("{roomId}")]
        public IActionResult GetGame([FromRoute] int roomId)
        {
            if (roomId == 0 || roomId > 1000)
                return BadRequest(roomId);
            if (!_gameStorage.DictionaryGame.ContainsKey(roomId))
                return NotFound();
            return Ok(_gameStorage.DictionaryGame[roomId]);
        }

        [HttpPost("bot")]
        public IActionResult GameWithBot([FromBody] GameActions gamerAction)
        {
            if (gamerAction == GameActions.None)
                return BadRequest();

            var result = _bot.PlayWithBot(gamerAction);

            return Ok(result);
        }
    }
}
