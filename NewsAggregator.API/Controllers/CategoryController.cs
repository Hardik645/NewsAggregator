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
            try
            {
                var result = await service.GetAllCategoryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving categories.", Details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] string name)
        {
            try
            {
                var result = await service.CreateCategoryAsync(name);
                return CreatedAtAction(nameof(GetCategory), new { id = result.CategoryId }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the category.", Details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var result = await service.GetCategoryWithKeywordsAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the category.", Details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await service.DeleteCategoryAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the category.", Details = ex.Message });
            }
        }

        [HttpPost("{id}/visibility")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetCategoryVisibility(int id, bool isHidden)
        {
            try
            {
                await service.SetCategoryVisibility(id, isHidden);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating category visibility.", Details = ex.Message });
            }
        }
    }
}



