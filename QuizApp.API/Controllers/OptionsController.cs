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
        public async Task<ActionResult<ApiResponse<List<OptionDto>>>> GetAll()
        {
            try
            {
                var options = await _context.Options
                    .Include(o => o.Question)
                    .Select(o => new OptionDto
                    {
                        Id = o.Id,
                        Text = o.Text,
                        IsCorrect = o.IsCorrect,
                        QuestionId = o.QuestionId
                    })
                    .ToListAsync();

                return Ok(ApiResponse<List<OptionDto>>.CreateSuccess(options, "Options retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<OptionDto>>.CreateFailure($"Error retrieving options: {ex.Message}"));
            }
        }

        // -----------------------------
        // GET: api/options/{id}
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OptionDto>>> GetById(int id)
        {
            try
            {
                var option = await _context.Options
                    .Include(o => o.Question)
                    .Where(o => o.Id == id)
                    .Select(o => new OptionDto
                    {
                        Id = o.Id,
                        Text = o.Text,
                        IsCorrect = o.IsCorrect,
                        QuestionId = o.QuestionId
                    })
                    .FirstOrDefaultAsync();

                if (option == null)
                    return NotFound(ApiResponse<OptionDto>.CreateFailure("Option not found."));

                return Ok(ApiResponse<OptionDto>.CreateSuccess(option, "Option retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<OptionDto>.CreateFailure($"Error retrieving option: {ex.Message}"));
            }
        }

        // -----------------------------
        // POST: api/options
        // -----------------------------
        [HttpPost]
        public async Task<ActionResult<ApiResponse<OptionDto>>> Create([FromBody] OptionDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Text))
                    return BadRequest(ApiResponse<OptionDto>.CreateFailure("Option text is required."));

                var question = await _context.Questions.FindAsync(dto.QuestionId);
                if (question == null)
                    return BadRequest(ApiResponse<OptionDto>.CreateFailure("Invalid QuestionId."));

                var option = new Option
                {
                    Text = dto.Text,
                    IsCorrect = dto.IsCorrect,
                    QuestionId = dto.QuestionId
                };

                _context.Options.Add(option);
                await _context.SaveChangesAsync();

                dto.Id = option.Id;
                return CreatedAtAction(nameof(GetById), new { id = dto.Id },
                    ApiResponse<OptionDto>.CreateSuccess(dto, "Option created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<OptionDto>.CreateFailure($"Error creating option: {ex.Message}"));
            }
        }

        // -----------------------------
        // PUT: api/options/{id}
        // -----------------------------
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] OptionDto dto)
        {
            try
            {
                var option = await _context.Options.FindAsync(id);
                if (option == null)
                    return NotFound(ApiResponse<object>.CreateFailure("Option not found."));

                if (string.IsNullOrWhiteSpace(dto.Text))
                    return BadRequest(ApiResponse<object>.CreateFailure("Option text is required."));

                var question = await _context.Questions.FindAsync(dto.QuestionId);
                if (question == null)
                    return BadRequest(ApiResponse<object>.CreateFailure("Invalid QuestionId."));

                option.Text = dto.Text;
                option.IsCorrect = dto.IsCorrect;
                option.QuestionId = dto.QuestionId;

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
