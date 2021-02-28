using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSPGame.Models.Game;
using RSPGame.Services.Rooms;

namespace RSPGame.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/rounds")]
    public class RoundController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoundController(IRoomService roomService)
        {
            _roomService = roomService;
        }
        
        [HttpGet("{roomId}")]
        public IActionResult GetCurrentRound([FromRoute] int roomId)
        {
            var roomRep = GetRoom(roomId);
            if (roomRep == null)
                return NoContent();

            var round = roomRep.GetCurrentRound();
            if (round == null)
                return NoContent();

            return Ok(round);
        }
        
        [HttpGet("complete/{roomId}")]
        public IActionResult TryGetCompletedRound([FromRoute] int roomId)
        {
            var roomRep = GetRoom(roomId);
            if (roomRep == null)
                return NoContent();

            var round = roomRep.TryGetCompleteRound();
            if (round == null)
                return NoContent();

            return Ok(round);
        }
        
        [HttpPost("action/{roomId}")]
        public IActionResult GamerAction([FromRoute] int roomId, [FromBody] GameRequest gameRequest)
        {
            if (gameRequest == null)
                return BadRequest();
            
            var roomRep = GetRoom(roomId);
            if (roomRep == null)
                return NoContent();
            try
            {
                roomRep.AddGamerAction(gameRequest.GamerInfo, gameRequest.Action);
            }
            catch (InvalidOperationException)
            {
                return Conflict();
            }

            return Ok();
        }

        [HttpPost("{roomId}")]
        public IActionResult AddRound([FromBody] Round round, [FromRoute] int roomId)
        {
            if (round == null)
                return BadRequest();
            
            var roomRep = GetRoom(roomId);
            if (roomRep == null)
                return NoContent();
            
            roomRep.SeriesRepository.AddRound(round);
            
            return Ok();
        }
        
        [HttpPut("ready/{roomId}")]
        public IActionResult SetReady([FromRoute] int roomId)
        {
            var roomRep = GetRoom(roomId);
            if (roomRep == null)
                return NoContent();
            
            roomRep.SetReady();
            
            return Ok();
        }
        
        [HttpPut("ready/check/{roomId}")]
        public IActionResult Check([FromRoute] int roomId)
        {
            var roomRep = GetRoom(roomId);
            if (roomRep == null)
                return NoContent();
            
            return Ok(roomRep.ReadyCounter == 2);
        }
        
        [HttpPut("refresh/{roomId}")]
        public IActionResult RefreshLastRound([FromRoute] int roomId)
        {
            var roomRep = GetRoom(roomId);
            if (roomRep == null)
                return NoContent();
            
            roomRep.RefreshCurrentRound();

            return Forbid();
        }

        private RoomRepository GetRoom(int roomId)
        {
            if (roomId < 0)
                return null;
            
            return _roomService.GetRoomRepById(roomId);
        }
    }
}
