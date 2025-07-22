using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Models;
using NewsAggregator.API.Services;
using NewsAggregator.API.Utils;

namespace NewsAggregator.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid input.");

                var (success, errorMessage) = await authService.SignupAsync(request);
                if (!success)
                    return Conflict(errorMessage);

                return Ok("Signup successful.");
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await authService.LoginAsync(request);
                if (!response.Success)
                    return Unauthorized(response.ErrorMessage);

                return Ok(new {response.UserId, response.Token, response.Role });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }
    }
}