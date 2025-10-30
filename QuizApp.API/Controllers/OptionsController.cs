using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.API.Data;
using QuizApp.API.Models;
using QuizApp.Shared.DTOs;

namespace QuizApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class OptionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -----------------------------
        // GET: api/options
        // -----------------------------
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Option>>>> GetAll()
        {
            try
            {
                var options = await _context.Options
                                            .Include(o => o.Question)
                                            .ToListAsync();
                return Ok(ApiResponse<List<Option>>.CreateSuccess(options, "Options retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<Option>>.CreateFailure($"Error retrieving options: {ex.Message}"));
            }
        }

        // -----------------------------
        // GET: api/options/{id}
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Option>>> GetById(int id)
        {
            try
            {
                var option = await _context.Options
                                           .Include(o => o.Question)
                                           .FirstOrDefaultAsync(o => o.Id == id);

                if (option == null)
                    return NotFound(ApiResponse<Option>.CreateFailure("Option not found."));

                return Ok(ApiResponse<Option>.CreateSuccess(option, "Option retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Option>.CreateFailure($"Error retrieving option: {ex.Message}"));
            }
        }

        // -----------------------------
        // POST: api/options
        // -----------------------------
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Option>>> Create([FromBody] Option option)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(option.Text))
                    return BadRequest(ApiResponse<Option>.CreateFailure("Option text is required."));

                var question = await _context.Questions.FindAsync(option.QuestionId);
                if (question == null)
                    return BadRequest(ApiResponse<Option>.CreateFailure("Invalid QuestionId."));

                _context.Options.Add(option);
                await _context.SaveChangesAsync();

                // Reload the option with related data
                var createdOption = await _context.Options
                                                  .Include(o => o.Question)
                                                  .FirstOrDefaultAsync(o => o.Id == option.Id);

                return CreatedAtAction(nameof(GetById), new { id = option.Id },
                    ApiResponse<Option>.CreateSuccess(createdOption, "Option created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Option>.CreateFailure($"Error creating option: {ex.Message}"));
            }
        }

        // -----------------------------
        // PUT: api/options/{id}
        // -----------------------------
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] Option updatedOption)
        {
            try
            {
                var option = await _context.Options.FindAsync(id);
                if (option == null)
                    return NotFound(ApiResponse<object>.CreateFailure("Option not found."));

                if (string.IsNullOrWhiteSpace(updatedOption.Text))
                    return BadRequest(ApiResponse<object>.CreateFailure("Option text is required."));

                // Validate QuestionId if it's being changed
                if (option.QuestionId != updatedOption.QuestionId)
                {
                    var question = await _context.Questions.FindAsync(updatedOption.QuestionId);
                    if (question == null)
                        return BadRequest(ApiResponse<object>.CreateFailure("Invalid QuestionId."));
                }

                option.Text = updatedOption.Text;
                option.IsCorrect = updatedOption.IsCorrect;
                option.QuestionId = updatedOption.QuestionId;

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.CreateSuccess(null, "Option updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.CreateFailure($"Error updating option: {ex.Message}"));
            }
        }

        // -----------------------------
        // DELETE: api/options/{id}
        // -----------------------------
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var option = await _context.Options.FindAsync(id);
                if (option == null)
                    return NotFound(ApiResponse<object>.CreateFailure("Option not found."));

                _context.Options.Remove(option);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.CreateSuccess(null, "Option deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.CreateFailure($"Error deleting option: {ex.Message}"));
            }
        }
    }
}