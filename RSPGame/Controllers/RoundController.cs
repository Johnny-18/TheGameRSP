using System;
using Microsoft.AspNetCore.Mvc;
using RSPGame.Models;
using RSPGame.Models.Game;
using RSPGame.Services;
using RSPGame.Services.Game;
using RSPGame.Storage;

namespace RSPGame.Controllers
{
    [ApiController]
    [Route("api/round")]
    public class RoundController : ControllerBase
    {
        private readonly RoundService _roundService;

        public RoundController(RoundService roundService)
        {
            _roundService = roundService;
        }
    }
}
