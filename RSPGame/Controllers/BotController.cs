using Microsoft.AspNetCore.Mvc;
using RSPGame.Models.GameModel;
using RSPGame.Services;

namespace RSPGame.Controllers
{
    [ApiController]
    [Route("api/bot")]
    public class BotController : ControllerBase
    {
        private readonly SinglePlayerService _bot;

        public BotController(SinglePlayerService bot)
        {
            _bot = bot;
        }

        [HttpPost]
        public IActionResult GameWithBot([FromBody] GameActions gamerAction)
        {
            if (gamerAction == GameActions.None)
                return BadRequest();

            var result = _bot.PlayWithBot(gamerAction);

            return Ok(result);
        }
    }
}
