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

<<<<<<< HEAD
            var result = _gameStorage.DictionaryGame.TryAdd(roomId, usersName);

            if (result) return Ok();
=======
            if (!ModelState.IsValid)
                return BadRequest(roomId);
            
            if (_gameStorage.DictionaryGame.TryAdd(roomId, usersName)) 
                return Ok();
            
>>>>>>> bfe52a06f44c3d4279d712b67031b5441d55857e
            return Conflict();
        }

        [HttpGet("{roomId}")]
        public IActionResult GetGame([FromRoute] int roomId)
        {
<<<<<<< HEAD
            if (roomId == 0 || roomId > 1000)
                return BadRequest(roomId);
            if (!_gameStorage.DictionaryGame.ContainsKey(roomId))
                return NotFound();
            return Ok(_gameStorage.DictionaryGame[roomId]);
=======
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
>>>>>>> bfe52a06f44c3d4279d712b67031b5441d55857e
        }
    }
}
