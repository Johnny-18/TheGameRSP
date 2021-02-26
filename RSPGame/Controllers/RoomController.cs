using Microsoft.AspNetCore.Mvc;
using RSPGame.Models.RoomModel;
using RSPGame.Services.Room;
using System;
using System.Threading.Tasks;
using RSPGame.Models.GameModel;

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
                return Ok(await _roomService.JoinRoom(gamer, id));
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        [HttpPost("stop")]
        public IActionResult PostStopRoom([FromBody] int roomId)
        {
            _roomService.DeleteRoom(roomId);
            return Ok();
        }
    }
}
