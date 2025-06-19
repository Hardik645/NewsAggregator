using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Models;
using NewsAggregator.API.Services;

namespace NewsAggregator.API.Controllers
{
    [ApiController]
    [Route("auth")]
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
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid input.");

                var (success, errorMessage) = await _authService.SignupAsync(request);
                if (!success)
                    return Conflict(errorMessage);

                return Ok("Signup successful.");
            }
            catch (Exception ex)
            {
                // Optionally log ex
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                if (!response.Success)
                    return Unauthorized(response.ErrorMessage);

                return Ok(new {response.UserId, response.Token, response.Role });
            }
            catch (Exception ex)
            {
                // Optionally log ex
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }
    }
}