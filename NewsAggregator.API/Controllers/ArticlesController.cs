using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Services;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Controllers
{
    [ApiController]
    [Route("api/articles")]
    [Authorize]
    public class ArticlesController(IArticleService articleService) : ControllerBase
    {
        private readonly IArticleService _articleService = articleService;

        [HttpPost("savedArticles")]
        public async Task<IActionResult> SaveArticle([FromQuery] int articleId)
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                bool result = await _articleService.SaveArticleAsync(articleId, userId);
                if (!result)
                    return Conflict("Article already saved");

                return Ok("Article saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("savedArticles")]
        public async Task<IActionResult> GetSavedArticles()
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                var saved = await _articleService.GetSavedArticlesAsync(userId);
                return Ok(saved);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("savedArticles")]
        public async Task<IActionResult> DeleteSavedArticle([FromQuery] int id)
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
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

        [HttpPost("{articleId}/feedback")]
        public async Task<IActionResult> SetArticleFeedback(int articleId, [FromQuery] bool isLike)
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                var result = await _articleService.SetArticleFeedbackAsync(articleId, userId, isLike);
                if (!result)
                    return BadRequest("Could not update feedback.");
                return Ok(isLike ? "Article liked." : "Article disliked.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{articleId}")]
        public async Task<IActionResult> GetArticleById(int articleId)
        {
            try
            {
                var article = await _articleService.GetArticleByIdAsync(articleId);
                if (article == null)
                    return NotFound();
                return Ok(article);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}