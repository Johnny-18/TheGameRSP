using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSPGame.Models;
using RSPGame.Services;

namespace RSPGame.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/game")]
    public class GameController : ControllerBase
    {
        private readonly SinglePlayerService _bot;

        public GameController(SinglePlayerService bot)
        {
            _bot = bot;
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
