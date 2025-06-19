using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Services;

namespace NewsAggregator.API.Controllers
{
    [ApiController]
    [Route("api/news")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodaysNews()
        {
            try
            {
                var articles = await _newsService.GetTodaysNewsAsync();
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
                var articles = await _newsService.GetNewsByDateRangeAsync(startDate, endDate);
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
                var articles = await _newsService.GetTodaysNewsByCategoryAsync(category);
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
                var articles = await _newsService.SearchNewsAsync(query, startDate, endDate, sortBy);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}