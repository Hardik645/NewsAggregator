using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Models;
using NewsAggregator.API.Services;

namespace NewsAggregator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input.");

            var (success, errorMessage) = await _authService.SignupAsync(request);
            if (!success)
                return Conflict(errorMessage);

            return Ok("Signup successful.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (success, token, role, errorMessage) = await _authService.LoginAsync(request);
            if (!success)
                return Unauthorized(errorMessage);

            return Ok(new { Token = token, Role = role });
        }
    }
}