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
        public IActionResult PostGame([FromBody] string[] usersName, [FromRoute] int roomId)
        {
            if (roomId == 0 || usersName.Any(string.IsNullOrWhiteSpace))
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(roomId);
            
            if (_gameStorage.DictionaryGame.TryAdd(roomId, usersName)) 
                return Ok();
            
            return Conflict();
        }

        [HttpGet("{roomId}")]
        public IActionResult GetGame()
        {
            var game = _gameStorage.DictionaryGame.FirstOrDefault();
            if (game.Value == null)
                return NoContent();
            
            return Ok();
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
