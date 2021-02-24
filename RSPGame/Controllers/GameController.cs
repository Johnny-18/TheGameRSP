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

            if (!ModelState.IsValid) return BadRequest(roomId);

            var result = _gameStorage.DictionaryGame.TryAdd(roomId, usersName);

            if (result) return Ok();
            return Conflict();
        }

        [HttpGet("{roomId}")]
        public IActionResult GetGame()
        {
            return Ok(_gameStorage.DictionaryGame.First());
        }
    }
}
