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
    public class QuestionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -----------------------------
        // GET: api/questions
        // -----------------------------
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<QuestionDto>>>> GetAll()
        {
            try
            {
                var questions = await _context.Questions
                    .Include(q => q.Category)
                    .Include(q => q.Options)
                    .Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        Text = q.Text,
                        CategoryId = q.CategoryId,
                        CategoryName = q.Category.Name,
                        Options = q.Options.Select(o => new OptionDto
                        {
                            Id = o.Id,
                            Text = o.Text,
                            IsCorrect = o.IsCorrect,
                            QuestionId = o.QuestionId
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(ApiResponse<List<QuestionDto>>.CreateSuccess(questions, "Questions retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<QuestionDto>>.CreateFailure($"Error retrieving questions: {ex.Message}"));
            }
        }

        // -----------------------------
        // GET: api/questions/{id}
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QuestionDto>>> GetById(int id)
        {
            try
            {
                var question = await _context.Questions
                    .Include(q => q.Category)
                    .Include(q => q.Options)
                    .Where(q => q.Id == id)
                    .Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        Text = q.Text,
                        CategoryId = q.CategoryId,
                        CategoryName = q.Category.Name,
                        Options = q.Options.Select(o => new OptionDto
                        {
                            Id = o.Id,
                            Text = o.Text,
                            IsCorrect = o.IsCorrect,
                            QuestionId = o.QuestionId
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (question == null)
                    return NotFound(ApiResponse<QuestionDto>.CreateFailure("Question not found."));

                return Ok(ApiResponse<QuestionDto>.CreateSuccess(question, "Question retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QuestionDto>.CreateFailure($"Error retrieving question: {ex.Message}"));
            }
        }

        // -----------------------------
        // POST: api/questions
        // -----------------------------
        [HttpPost]
        public async Task<ActionResult<ApiResponse<QuestionDto>>> Create([FromBody] QuestionDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Text))
                    return BadRequest(ApiResponse<QuestionDto>.CreateFailure("Question text is required."));

                var category = await _context.Categories.FindAsync(dto.CategoryId);
                if (category == null)
                    return BadRequest(ApiResponse<QuestionDto>.CreateFailure("Invalid category ID."));

                if (dto.Options == null || dto.Options.Count < 2)
                    return BadRequest(ApiResponse<QuestionDto>.CreateFailure("At least two options are required."));

                if (!dto.Options.Any(o => o.IsCorrect))
                    return BadRequest(ApiResponse<QuestionDto>.CreateFailure("At least one correct option is required."));

                var question = new Question
                {
                    Text = dto.Text,
                    CategoryId = dto.CategoryId,
                    Options = dto.Options.Select(o => new Option
                    {
                        Text = o.Text,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                };

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                dto.Id = question.Id;
                dto.CategoryName = category.Name;

                return CreatedAtAction(nameof(GetById), new { id = question.Id },
                    ApiResponse<QuestionDto>.CreateSuccess(dto, "Question created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QuestionDto>.CreateFailure($"Error creating question: {ex.Message}"));
            }
        }

        // -----------------------------
        // PUT: api/questions/{id}
        // -----------------------------
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] QuestionDto dto)
        {
            try
            {
                var question = await _context.Questions
                    .Include(q => q.Options)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (question == null)
                    return NotFound(ApiResponse<object>.CreateFailure("Question not found."));

                if (string.IsNullOrWhiteSpace(dto.Text))
                    return BadRequest(ApiResponse<object>.CreateFailure("Question text is required."));

                var category = await _context.Categories.FindAsync(dto.CategoryId);
                if (category == null)
                    return BadRequest(ApiResponse<object>.CreateFailure("Invalid category ID."));

                if (dto.Options == null || dto.Options.Count < 2)
                    return BadRequest(ApiResponse<object>.CreateFailure("At least two options are required."));

                if (!dto.Options.Any(o => o.IsCorrect))
                    return BadRequest(ApiResponse<object>.CreateFailure("At least one correct option is required."));

                // Update main question
                question.Text = dto.Text;
                question.CategoryId = dto.CategoryId;

                // Replace options
                _context.Options.RemoveRange(question.Options);
                question.Options = dto.Options.Select(o => new Option
                {
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList();

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.CreateSuccess(null, "Question updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.CreateFailure($"Error updating question: {ex.Message}"));
            }
        }

        // -----------------------------
        // DELETE: api/questions/{id}
        // -----------------------------
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var question = await _context.Questions
                    .Include(q => q.Options)
                    .Include(q => q.QuizQuestions)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (question == null)
                    return NotFound(ApiResponse<object>.CreateFailure("Question not found."));

                if (question.QuizQuestions.Any())
                    return BadRequest(ApiResponse<object>.CreateFailure("Cannot delete question that is assigned to quizzes. Remove from quizzes first."));

                _context.Options.RemoveRange(question.Options);
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.CreateSuccess(null, "Question deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.CreateFailure($"Error deleting question: {ex.Message}"));
            }
        }
    }
}
