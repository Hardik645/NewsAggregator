using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Models;
using NewsAggregator.API.Services;
using NewsAggregator.API.Utils;

namespace NewsAggregator.API.Controllers
{
    [ApiController]
    [Route("api/keywords")]
    public class KeywordController(IKeywordService service) : ControllerBase
    {
        [HttpPost("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddKeywords(int id, [FromBody] CreateKeywordRequest request)
        {
            try
            {
                await service.AddKeywordsAsync(id, request);
                return Ok(new { Message = "Keywords added successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { Message = "An error occurred while adding keywords.", Details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteKeyword(int id)
        {
            try
            {
                await service.DeleteKeywordAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { Message = "An error occurred while deleting the keyword.", Details = ex.Message });
            }
        }

        [HttpPost("hidden")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddHiddenKeyword([FromBody] string keyword)
        {
            try
            {
                await service.AddHiddenKeywordAsync(keyword);
                await service.HideArticlesWithHiddenKeywordsAsync();
                return Ok(new { Message = "Keyword hidden successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { Message = "An error occurred while hiding the keyword.", Details = ex.Message });
            }
        }

        [HttpDelete("hidden")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveHiddenKeyword([FromQuery] string keyword)
        {
            try
            {
                await service.RemoveHiddenKeywordAsync(keyword);
                await service.HideArticlesWithHiddenKeywordsAsync();
                return Ok(new { Message = "Keyword unhidden successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { Message = "An error occurred while unhiding the keyword.", Details = ex.Message });
            }
        }
        
        [HttpGet("hidden")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllHiddenKeywords()
        {
            try
            {
                var result = await service.GetAllHiddenKeywordsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { Message = "An error occurred while retrieving hidden keywords.", Details = ex.Message });
            }
        }
    }
}
