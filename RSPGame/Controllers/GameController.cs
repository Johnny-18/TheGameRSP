using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RSPGame.Models;
using RSPGame.Storage;

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

        [HttpPost("/{roomId}")]
        public async Task<IActionResult> PostGame([FromRoute] int roomId, [FromBody] string userName1, [FromBody] string userName2)
        {

            _gameStorage.DictionaryGame.TryAdd(roomId, new[] {userName1, userName2});

            return Ok();
        }
    }
}
