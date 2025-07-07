using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Services;
using NewsAggregator.API.Utils;

namespace NewsAggregator.API.Controllers
{
    [ApiController]
    [Route("api/news")]
    [Authorize]
    public class NewsController(INewsService newsService, IRecommendationService recommendationService) : ControllerBase
    {
        [HttpGet("today")]
        public async Task<IActionResult> GetTodaysNews([FromQuery] string? category)
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                var articles = await newsService.GetTodaysNewsAsync(category);
                var recommendationalArticles = await recommendationService.SortArticlesByUserScoreAsync(userId, articles);
                return Ok(recommendationalArticles);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
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
                var userId = UserContextHelper.GetUserId(User);
                var articles = await newsService.GetNewsByDateRangeAsync(startDate, endDate, category);
                var recommendationalArticles = await recommendationService.SortArticlesByUserScoreAsync(userId, articles);
                return Ok(recommendationalArticles);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
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
                ExceptionLogger.LogException(ex);
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
                var userId = UserContextHelper.GetUserId(User);
                var articles = await newsService.SearchNewsAsync(query, startDate, endDate, sortBy);
                var recommendationalArticles = await recommendationService.SortArticlesByUserScoreAsync(userId, articles);
                return Ok(recommendationalArticles);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}