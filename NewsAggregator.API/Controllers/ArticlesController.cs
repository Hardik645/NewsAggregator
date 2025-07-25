using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Services;
using NewsAggregator.API.Utils;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Controllers
{
    [ApiController]
    [Route("api/articles")]
    [Authorize]
    public class ArticlesController(IArticleService articleService, IUserActionLoggingService userActionLoggingService) : ControllerBase
    {

        [HttpPost("saveArticle")]
        public async Task<IActionResult> SaveArticle([FromQuery] int articleId)
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                bool result = await articleService.SaveArticleAsync(articleId, userId);
                if (!result)
                    return Conflict("Article already saved");
                await userActionLoggingService.LogActionAsync(userId, articleId, "Save");
                return Ok("Article saved successfully.");
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("savedArticles")]
        public async Task<IActionResult> GetSavedArticles()
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                var saved = await articleService.GetSavedArticlesAsync(userId);
                return Ok(saved);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("savedArticles")]
        public async Task<IActionResult> DeleteSavedArticle([FromQuery] int id)
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                bool result = await articleService.DeleteSavedArticleAsync(id, userId);
                if (!result)
                    return BadRequest("Could not delete article.");
                
                return Ok("Article deleted successfully.");
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{articleId}/feedback")]
        public async Task<IActionResult> SetArticleFeedback(int articleId, [FromQuery] bool? isLike, [FromQuery] bool? isReported)
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                var result = await articleService.SetArticleFeedbackAsync(articleId, userId, isLike, isReported);
                if (!result)
                    return BadRequest("Could not update feedback.");

                if (isReported.HasValue && isReported.Value)
                    return Ok("Article reported.");
                if (isLike.HasValue)
                {
                    if(isLike.Value)
                        await userActionLoggingService.LogActionAsync(userId, articleId, "Like");
                    return Ok(isLike.Value ? "Article liked." : "Article disliked.");
                }
                return Ok("Feedback updated.");
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{articleId}")]
        public async Task<IActionResult> GetArticleById(int articleId)
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                var article = await articleService.GetArticleByIdAsync(articleId);
                if (article == null)
                    return NotFound();
                await userActionLoggingService.LogActionAsync(userId, articleId, "View");
                return Ok(article);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("hideArticles/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HideArticles(int id)
        {
            try
            {
                await articleService.HideArticlesAsync(id);
                return Ok(new { Message = $"Articles marked as hidden." });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { Message = "An error occurred while hiding articles.", Details = ex.Message });
            }
        }

        [HttpGet("reportedArticles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllReportedArticles()
        {
            try
            {
                var articles = await articleService.GetAllReportedNotHiddenArticlesAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, new { Message = "An error occurred while retrieving articles.", Details = ex.Message });
            }
        }
    }
}