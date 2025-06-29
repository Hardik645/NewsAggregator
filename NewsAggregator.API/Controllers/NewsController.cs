using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Services;

namespace NewsAggregator.API.Controllers
{
    [ApiController]
    [Route("api/news")]
    [Authorize]
    public class NewsController(INewsService newsService) : ControllerBase
    {
        [HttpGet("today")]
        public async Task<IActionResult> GetTodaysNews([FromQuery] string? category)
        {
            try
            {
                var articles = await newsService.GetTodaysNewsAsync(category);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetNewsByDateRange(
            [FromQuery] DateOnly startDate,
            [FromQuery] DateOnly endDate,
            [FromQuery] string? category)
        {
            try
            {
                var articles = await newsService.GetNewsByDateRangeAsync(startDate, endDate, category);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetTodaysNewsByCategory(string category)
        {
            try
            {
                var articles = await newsService.GetTodaysNewsByCategoryAsync(category);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchNews(
            [FromQuery] string query,
            [FromQuery] DateOnly? startDate,
            [FromQuery] DateOnly? endDate,
            [FromQuery] string? sortBy)
        {
            try
            {
                var articles = await newsService.SearchNewsAsync(query, startDate, endDate, sortBy);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}