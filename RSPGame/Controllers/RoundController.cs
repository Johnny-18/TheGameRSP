using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RSPGame.Storage;
using System.Linq;
using RSPGame.Models.GameModel;
using RSPGame.Services.Rsp;

namespace RSPGame.Controllers
{
    [ApiController]
    [Route("api/round")]
    public class RoundController : ControllerBase
    {
        private readonly RoundStorage _roundStorage;
        private readonly IRspService _rspService;

        public RoundController(RoundStorage roundStorage, IRspService rspService)
        {
            _roundStorage = roundStorage;
            _rspService = rspService;
        }

        [HttpPost("{roomId}/{userName}")]
        public IActionResult PostRound([FromBody] GameActions action, [FromRoute] string userName, [FromRoute] int roomId)
        {
            if (roomId < 1 || roomId > 1000)
                return BadRequest(roomId);

            var gamerReq = new GamerStep
            {
                UserName = userName,
                UserAction = action
            };

            _roundStorage.AddGamer(roomId, gamerReq);

            return Ok();
        }

        [HttpGet("{roomId}/{userName}")]
        public IActionResult GetRound([FromRoute] string userName, [FromRoute] int roomId)
        {
            if (roomId < 1 || roomId > 1000)
                return BadRequest(roomId);

            if (!_roundStorage.ContainRoom(roomId))
                return NotFound(roomId);

            var gamers = _roundStorage.PeekGamers(roomId).ToArray();

            if (gamers.Length != 2)
                return Conflict();

            var gamer1 = gamers.First();
            var gamer2 = gamers.Last();

            var result = _rspService.GetWinner(gamer1.UserAction, gamer2.UserAction);

            if (gamer1.UserName == userName)
                return Ok(result);

            return Ok(_rspService.InverseResult(result));
        }

        [HttpDelete("{roomId}")]
        public IActionResult DeleteSeries([FromRoute] int roomId)
        {
            _roundStorage.DeleteGamers(roomId);
            return Ok();
        }

        [HttpPost("{roomId}")]
        public IActionResult SetReady([FromRoute] int roomId, [FromBody] string userName)
        {
            if (!_roundStorage.ContainRoom(roomId))
                return NotFound();

            var gamer = new GamerStep()
            {
                UserName = userName,
                UserAction = GameActions.Ready
            };

            _roundStorage.AddGamer(roomId, gamer);
            return Ok();
        }

        [HttpGet("{roomId}")]
        public IActionResult IsReady([FromRoute] int roomId)
        {
            IEnumerable<GamerStep> gamers;

            try
            {
                gamers = _roundStorage.PeekGamers(roomId).ToArray();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }

            if (gamers == null || gamers.Count() != 2)
                return NotFound();

            var result = gamers.All(x => x.UserAction == GameActions.Ready);

            if (result)
                return Ok(result);

            return NotFound();
        }
    }
}
