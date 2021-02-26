using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RSPGame.Models;
using RSPGame.Models.RoomModel;
using RSPGame.Services.Room;
using RSPGame.Storage;

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
        public async Task<IActionResult> FindPublicRoom([FromBody] GamerInfo gamer)
        {
            if (gamer == null)
                return BadRequest();

            var result = await _roomService.JoinRoom(gamer);

            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePrivateRoom([FromBody] GamerInfo gamer)
        {
            if (gamer == null)
                return BadRequest();

            var result = await _roomService.CreateRoom(gamer, RoomStatus.Private);

            return Ok(result);
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinPrivateRoom([FromBody] GamerInfo gamer, [FromQuery] int id)
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

        [HttpPost("stop")]
        public IActionResult PostStopRoom([FromBody] int roomId)
        {
            _roomService.DeleteRoom(roomId);
            return Ok();
        }
    }
}
