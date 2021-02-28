using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RSPGame.Models;
using RSPGame.Services.Rooms;

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

        [HttpGet("{roomId}")]
        public IActionResult GetRoomById([FromRoute] int roomId)
        {
            if (roomId < 0)
                return BadRequest();

            var roomRep = _roomService.GetRoomRepById(roomId);
            if (roomRep == null)
            {
                return NotFound();
            }

            return Ok(roomRep);
        }

        [HttpGet("gamers/{roomId}")]
        public IActionResult GetGamerFromRoom([FromRoute] int roomId)
        {
            if (roomId < 0)
                return BadRequest();

            var roomRep = _roomService.GetRoomRepById(roomId);
            if (roomRep == null)
            {
                return NotFound();
            }

            return Ok(roomRep.GetGamers().ToArray());
        }

        [HttpGet("save/{roomId}")]
        public IActionResult SaveWork([FromRoute] int roomId)
        {
            if (roomId < 0)
                return BadRequest();

            _roomService.SaveStatForGamersAsync(roomId);
            
            return Ok();
        }

        [HttpPost("join")]
        public IActionResult JoinRoom([FromBody] GamerInfo gamer, [FromQuery] int id)
        {
            if (gamer == null)
                return BadRequest();

            try
            {
                return Ok(_roomService.JoinRoom(gamer, id));
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRoom([FromBody] GamerInfo gamer)
        {
            if (gamer == null)
                return BadRequest();

            var result = await _roomService.CreateRoomAsync(gamer, true);

            return Ok(result);
        }

        [HttpDelete("{roomId}")]
        public IActionResult DeleteRoom([FromRoute] int roomId)
        {
            if (roomId < 0)
                return BadRequest();

            if (_roomService.RemoveRoom(roomId))
            {
                return Ok();
            }
            
            return NoContent();
        }
    }
}
