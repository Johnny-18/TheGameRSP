using Microsoft.AspNetCore.Mvc;
using RSPGame.Storage;
using System.Linq;

namespace RSPGame.Controllers
{
    [ApiController]
    [Route("api/game")]
    public class GameController : ControllerBase
    {
        private readonly GameStorage _gameStorage;

        public GameController(GameStorage gameStorage)
        {
            _gameStorage = gameStorage;
        }

        [HttpPost("{roomId}")]
        public IActionResult PostGame([FromBody] string[] usersName, [FromRoute] int roomId)
        {
            if (roomId == 0 || usersName.Any(string.IsNullOrWhiteSpace))
                return BadRequest();

            var result = _gameStorage.DictionaryGame.TryAdd(roomId, usersName);

            if (result) return Ok();
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
    }
}
