using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RSPGame.Models;
using RSPGame.Services.Authentication;

namespace RSPGame.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] RequestUser user)
        {
            if (user == null)
                return BadRequest();

            var session = await _authService.LoginAsync(user);
            if (session == null)
                return Forbid();

            return Ok(session);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RequestUser user)
        {
            if (user == null)
                return BadRequest();

            var session = await _authService.RegisterAsync(user);
            if (session == null)
                return Forbid();
            
            return Ok(session);
        }
    }
}