using Microsoft.AspNetCore.Mvc;
using RSPGame.Models.RoomModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using RSPGame.Models.GameModel;
using RSPGame.Services.RoomService;

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
        public async Task<IActionResult> CreateRoom([FromBody] GamerInfo gamer)
        {
            if (gamer == null)
                return BadRequest();

            var result = await _roomService.CreateRoom(gamer, RoomStatus.Private);

            return Ok(result);
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinRoom([FromBody] GamerInfo gamer, [FromQuery] int id)
        {
            if (gamer == null)
                return BadRequest();

            try
            {
                var result = await _roomService.JoinRoom(gamer, id);
                if (result == 0) return NotFound();

                return Ok(result);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        [HttpGet("{roomId}")]
        public IActionResult GetRoom([FromRoute] int roomId)
        {
            if (roomId == 0 || roomId > 1000)
                return BadRequest(roomId);

            var room = _roomService.GetRoom(roomId);

            if (room == null || room.IsFree())
                return NotFound();
            
            return Ok(room.GetGamers.Select(x => x.UserName).ToArray());
        }

        [HttpDelete("stop/{roomId}")]
        public IActionResult DeleteRoom([FromRoute] int roomId)
        {
            _roomService.DeleteRoom(roomId);
            return Ok();
        }
    }
}
