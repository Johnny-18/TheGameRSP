using Microsoft.AspNetCore.Mvc;
using RSPGame.Models;
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

        [HttpPost]
        public IActionResult PostGame([FromBody] Game game)
        {
            if (game.RoomId == 0 || game.UsersName.Any(string.IsNullOrWhiteSpace))
                return BadRequest();

            if (!ModelState.IsValid) return BadRequest(game.RoomId);

            var result = _gameStorage.DictionaryGame.TryAdd(game.RoomId, game.UsersName);

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
