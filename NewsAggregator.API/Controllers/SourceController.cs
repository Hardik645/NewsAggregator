using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Models;
using NewsAggregator.API.Services;
using NewsAggregator.API.Utils;

namespace NewsAggregator.API.Controllers
{
    [Route("api/admin/source")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SourceController(ISourceService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, $"An error occurred while retrieving sources: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await service.GetByIdAsync(id);
                return result != null ? Ok(result) : Ok("Not Found");
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, $"An error occurred while retrieving the source: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SourceRequest dto)
        {
            try
            {
                var result = await service.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, $"An error occurred while adding the source: {ex.Message}");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SourceUpdateRequest dto)
        {
            try
            {
                var result = await service.UpdateAsync(id, dto);
                return result != null ? Ok(result) : NotFound();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, $"An error occurred while updating the source: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await service.DeleteAsync(id);
                return success ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return StatusCode(500, $"An error occurred while deleting the source: {ex.Message}");
            }
        }
    }
}
