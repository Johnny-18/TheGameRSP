using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RSPGame.Models;
using RSPGame.Services;

namespace RSPGame.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost("find")]
        public async Task<IActionResult> FindPublicGame([FromBody] GamerInfo gamer)
        {
            if (gamer == null)
                return BadRequest();

            await _roomService.JoinRoom(gamer);

            return Ok();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePrivateGame([FromBody] GamerInfo gamer)
        {
            if (gamer == null)
                return BadRequest();

            await _roomService.CreateRoom(gamer, RoomStatus.Private);

            return Ok();
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinPrivateGame([FromBody] GamerInfo gamer, [FromQuery] int id)
        {
            if (gamer == null)
                return BadRequest();

            try
            {
                await _roomService.JoinRoom(gamer, id);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }

            return Ok();
        }


    }
}
