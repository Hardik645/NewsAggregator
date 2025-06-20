using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Models;
using NewsAggregator.API.Services;

namespace NewsAggregator.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController(ICategoryService service) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCategory()
        {
            var result = await service.GetAllCategoryAsync();
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] string name)
        {
            var result = await service.CreateCategoryAsync(name);
            return CreatedAtAction(nameof(GetCategory), new { id = result.CategoryId }, result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCategory(int id)
        {
            var result = await service.GetCategoryWithKeywordsAsync(id);
            return Ok(result);
        }

        [HttpPost("{id}/keywords")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddKeywords(int id, [FromBody] CreateKeywordRequest request)
        {
            await service.AddKeywordsAsync(id, request);
            return Ok(new { Message = "Keywords added successfully." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await service.DeleteCategoryAsync(id);
            return NoContent();
        }

        [HttpDelete("keywords/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteKeyword(int id)
        {
            await service.DeleteKeywordAsync(id);
            return NoContent();
        }
    }
}
