using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.API.Models;
using NewsAggregator.API.Services;

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
            var result = await service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await service.GetByIdAsync(id);
            return result != null ? Ok(result) : Ok("Not Found");
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SourceRequest dto)
        {
            var result = await service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SourceUpdateRequest dto)
        {
            var result = await service.UpdateAsync(id, dto);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
    
}
