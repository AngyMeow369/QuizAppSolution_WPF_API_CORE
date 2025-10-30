using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<ApiResponse<List<Question>>>> GetAll()
        {
            try
            {
                var questions = await _context.Questions
                                              .Include(q => q.Category)
                                              .Include(q => q.Options)
                                              .ToListAsync();
                return Ok(ApiResponse<List<Question>>.CreateSuccess(questions, "Questions retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<Question>>.CreateFailure($"Error retrieving questions: {ex.Message}"));
            }
        }

        // -----------------------------
        // GET: api/questions/{id}
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Question>>> GetById(int id)
        {
            try
            {
                var question = await _context.Questions
                                             .Include(q => q.Category)
                                             .Include(q => q.Options)
                                             .FirstOrDefaultAsync(q => q.Id == id);

                if (question == null)
                    return NotFound(ApiResponse<Question>.CreateFailure("Question not found."));

                return Ok(ApiResponse<Question>.CreateSuccess(question, "Question retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Question>.CreateFailure($"Error retrieving question: {ex.Message}"));
            }
        }

        // -----------------------------
        // POST: api/questions
        // -----------------------------
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Question>>> Create([FromBody] Question question)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(question.Text))
                    return BadRequest(ApiResponse<Question>.CreateFailure("Question text is required."));

                var category = await _context.Categories.FindAsync(question.CategoryId);
                if (category == null)
                    return BadRequest(ApiResponse<Question>.CreateFailure("Invalid category ID."));

                // Validate that at least one option is marked as correct
                if (question.Options != null && question.Options.Any())
                {
                    var hasCorrectOption = question.Options.Any(o => o.IsCorrect);
                    if (!hasCorrectOption)
                        return BadRequest(ApiResponse<Question>.CreateFailure("At least one option must be marked as correct."));
                }

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                // Reload the question with related data
                var createdQuestion = await _context.Questions
                                                    .Include(q => q.Category)
                                                    .Include(q => q.Options)
                                                    .FirstOrDefaultAsync(q => q.Id == question.Id);

                return CreatedAtAction(nameof(GetById), new { id = question.Id },
                    ApiResponse<Question>.CreateSuccess(createdQuestion, "Question created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Question>.CreateFailure($"Error creating question: {ex.Message}"));
            }
        }

        // -----------------------------
        // PUT: api/questions/{id}
        // -----------------------------
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] Question updatedQuestion)
        {
            try
            {
                var question = await _context.Questions
                                             .Include(q => q.Options)
                                             .FirstOrDefaultAsync(q => q.Id == id);

                if (question == null)
                    return NotFound(ApiResponse<object>.CreateFailure("Question not found."));

                if (string.IsNullOrWhiteSpace(updatedQuestion.Text))
                    return BadRequest(ApiResponse<object>.CreateFailure("Question text is required."));

                var category = await _context.Categories.FindAsync(updatedQuestion.CategoryId);
                if (category == null)
                    return BadRequest(ApiResponse<object>.CreateFailure("Invalid category ID."));

                question.Text = updatedQuestion.Text;
                question.CategoryId = updatedQuestion.CategoryId;

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

                // Check if question is used in any quizzes
                if (question.QuizQuestions.Any())
                    return BadRequest(ApiResponse<object>.CreateFailure("Cannot delete question that is assigned to quizzes. Remove from quizzes first."));

                // Remove related options first
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