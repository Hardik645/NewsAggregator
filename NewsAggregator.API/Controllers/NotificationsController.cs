using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Models;
using NewsAggregator.API.Services;
using NewsAggregator.API.Utils;

namespace NewsAggregator.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController(INotificationService notificationService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                var notifications = await notificationService.GetUserNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, $"Failed to get notifications: {ex.Message}");
            }
        }

        [HttpGet("config")]
        public async Task<IActionResult> GetConfig()
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                var config = await notificationService.GetUserNotificationConfigAsync(userId);
                return Ok(new { categories = config.Categories, keywords = config.Keywords });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, $"Failed to get notification config: {ex.Message}");
            }
        }

        [HttpPut("config/category")]
        public async Task<IActionResult> SetCategory([FromBody] CategoryConfigDto dto)
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                var result = await notificationService.SetCategoryNotificationAsync(userId, dto.Category, dto.Enabled);
                if (!result) return BadRequest("Category not found.");
                return Ok();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, $"Failed to update category notification: {ex.Message}");
            }
        }

        [HttpPut("config/keywords")]
        public async Task<IActionResult> SetKeywords([FromBody] KeywordsConfigDto dto)
        {
            try
            {
                var userId = UserContextHelper.GetUserId(User);
                await notificationService.SetKeywordNotificationsAsync(userId, dto.Keywords);
                return Ok();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, $"Failed to update keyword notifications: {ex.Message}");
            }
        }
    }
}       