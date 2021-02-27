using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RSPGame.Models;
using RSPGame.Models.Game;
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

        [HttpPost("join")]
        public IActionResult JoinPrivateRoom([FromBody] GamerInfo gamer, [FromQuery] int id)
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

        [HttpPost("create")]
        public async Task<IActionResult> CreatePrivateRoom([FromBody] GamerInfo gamer)
        {
            if (gamer == null)
                return BadRequest();

            var result = await _roomService.CreateRoomAsync(gamer, true);

            return Ok(result);
        }

        [HttpPost("{roomId}/action")]
        public IActionResult GamerAction([FromRoute] int roomId, [FromBody] GameRequest gameRequest)
        {
            if (roomId < 0 || gameRequest == null)
                return BadRequest();

            var roomRep = _roomService.GetRoomRepById(roomId);
            if (roomRep == null)
                return NotFound();
            
            roomRep.AddGamerAction(gameRequest.GamerInfo, gameRequest.Action);

            return Ok();
        }

        [HttpGet("{roomId}/lastRound")]
        public IActionResult GetLastRound([FromRoute] int roomId)
        {
            if (roomId < 0)
                return BadRequest();

            var roomRep = _roomService.GetRoomRepById(roomId);
            if (roomRep == null)
                return NotFound();

            var round = roomRep.SeriesRepository.GetLastRound();
            if (round == null)
                return NoContent();

            return Ok(round);
        }

        [HttpDelete("{roomId}/laseRound")]
        public IActionResult DeleteLastRound([FromRoute] int roomId)
        {
            if (roomId < 0)
                return BadRequest();
            
            var roomRep = _roomService.GetRoomRepById(roomId);
            if (roomRep == null)
                return NotFound();
            
            var round = roomRep.SeriesRepository.RemoveLastRound();
            if (round == null)
                return NoContent();

            return Ok(round);
        }
    }
}
