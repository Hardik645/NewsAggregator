using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Services;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Controllers
{
    [ApiController]
    [Route("api/articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticlesController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveArticle([FromQuery] int articleId, [FromQuery] Guid userId)
        {
            try
            {
                bool result = await _articleService.SaveArticleAsync(articleId, userId);
                if (!result)
                    return BadRequest("Could not save article.");

                return Ok("Article saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetSavedArticles(Guid userId)
        {
            try
            {
                var saved = await _articleService.GetSavedArticlesAsync(userId);
                return Ok(saved);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}/{userId}")]
        public async Task<IActionResult> DeleteSavedArticle(int id, Guid userId)
        {
            try
            {
                bool result = await _articleService.DeleteSavedArticleAsync(id, userId);
                if (!result)
                    return BadRequest("Could not delete article.");
                
                return Ok("Article deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}