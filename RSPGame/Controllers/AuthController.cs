using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

            var session = await _authService.Login(user);
            if (session == null)
                return NotFound();
            
            return Ok(session);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RequestUser user)
        {
            if (user == null)
                return BadRequest();

            var session = await _authService.Register(user);
            if (session == null)
                return NotFound();
            
            return Ok(session);
        }
        
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout([FromBody] Session session)
        {
            if (session == null)
                return BadRequest();

            _authService.Logout(session);

            return Ok(session);
        }
    }
}