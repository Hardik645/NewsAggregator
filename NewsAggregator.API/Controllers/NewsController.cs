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
        public async Task<IActionResult> GetTodaysNews()
        {
            try
            {
                var articles = await newsService.GetTodaysNewsAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetNewsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var articles = await newsService.GetNewsByDateRangeAsync(startDate, endDate);
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
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
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