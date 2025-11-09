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
    public class QuizzesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuizzesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/quizzes
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<QuizDto>>>> GetAll()
        {
            try
            {
                var quizzes = await _context.Quizzes
                    .Include(q => q.Category)
                    .Include(q => q.QuizQuestions)
                        .ThenInclude(qq => qq.Question)
                    .ToListAsync();

                var quizDtos = quizzes.Select(q => new QuizDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    CategoryId = q.CategoryId,
                    CategoryName = q.Category?.Name ?? string.Empty,
                    StartTime = q.StartTime,
                    EndTime = q.EndTime,
                    QuestionIds = q.QuizQuestions.Select(qq => qq.QuestionId).ToList()
                }).ToList();

                return Ok(ApiResponse<List<QuizDto>>.CreateSuccess(quizDtos, "Quizzes retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<QuizDto>>.CreateFailure($"Error retrieving quizzes: {ex.Message}"));
            }
        }

        // GET: api/quizzes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QuizDto>>> GetById(int id)
        {
            try
            {
                var quiz = await _context.Quizzes
                    .Include(q => q.Category)
                    .Include(q => q.QuizQuestions)
                        .ThenInclude(qq => qq.Question)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quiz == null)
                    return NotFound(ApiResponse<QuizDto>.CreateFailure("Quiz not found."));

                var dto = new QuizDto
                {
                    Id = quiz.Id,
                    Title = quiz.Title,
                    StartTime = quiz.StartTime,
                    EndTime = quiz.EndTime,
                    CategoryId = quiz.CategoryId,
                    CategoryName = quiz.Category?.Name ?? string.Empty,
                    QuestionIds = quiz.QuizQuestions.Select(qq => qq.QuestionId).ToList()
                };

                return Ok(ApiResponse<QuizDto>.CreateSuccess(dto, "Quiz retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QuizDto>.CreateFailure($"Error retrieving quiz: {ex.Message}"));
            }
        }

        // POST: api/quizzes
        [HttpPost]
        public async Task<ActionResult<ApiResponse<QuizDto>>> Create([FromBody] QuizDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Title))
                    return BadRequest(ApiResponse<QuizDto>.CreateFailure("Quiz title is required."));

                if (dto.StartTime >= dto.EndTime)
                    return BadRequest(ApiResponse<QuizDto>.CreateFailure("End time must be after start time."));

                var category = await _context.Categories.FindAsync(dto.CategoryId);
                if (category == null)
                    return BadRequest(ApiResponse<QuizDto>.CreateFailure("Invalid category ID."));

                var quiz = new Quiz
                {
                    Title = dto.Title,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    CategoryId = dto.CategoryId
                };

                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();

                // handle question mapping
                if (dto.QuestionIds != null && dto.QuestionIds.Any())
                {
                    foreach (var qid in dto.QuestionIds)
                        _context.QuizQuestions.Add(new QuizQuestion { QuizId = quiz.Id, QuestionId = qid });

                    await _context.SaveChangesAsync();
                }

                dto.Id = quiz.Id;
                return CreatedAtAction(nameof(GetById), new { id = quiz.Id },
                    ApiResponse<QuizDto>.CreateSuccess(dto, "Quiz created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QuizDto>.CreateFailure($"Error creating quiz: {ex.Message}"));
            }
        }

        // PUT: api/quizzes/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] QuizDto dto)
        {
            try
            {
                var quiz = await _context.Quizzes
                    .Include(q => q.QuizQuestions)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quiz == null)
                    return NotFound(ApiResponse<object>.CreateFailure("Quiz not found."));

                if (string.IsNullOrWhiteSpace(dto.Title))
                    return BadRequest(ApiResponse<object>.CreateFailure("Quiz title is required."));

                if (dto.StartTime >= dto.EndTime)
                    return BadRequest(ApiResponse<object>.CreateFailure("End time must be after start time."));

                quiz.Title = dto.Title;
                quiz.StartTime = dto.StartTime;
                quiz.EndTime = dto.EndTime;
                quiz.CategoryId = dto.CategoryId;

                // Update question links
                _context.QuizQuestions.RemoveRange(quiz.QuizQuestions);
                if (dto.QuestionIds != null && dto.QuestionIds.Any())
                {
                    foreach (var qid in dto.QuestionIds)
                        _context.QuizQuestions.Add(new QuizQuestion { QuizId = quiz.Id, QuestionId = qid });
                }

                await _context.SaveChangesAsync();
                return Ok(ApiResponse<object>.CreateSuccess(null, "Quiz updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.CreateFailure($"Error updating quiz: {ex.Message}"));
            }
        }

        // DELETE: api/quizzes/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var quiz = await _context.Quizzes
                    .Include(q => q.QuizQuestions)
                    .Include(q => q.Assignments)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quiz == null)
                    return NotFound(ApiResponse<object>.CreateFailure("Quiz not found."));

                _context.QuizQuestions.RemoveRange(quiz.QuizQuestions);
                _context.QuizAssignments.RemoveRange(quiz.Assignments);
                _context.Quizzes.Remove(quiz);

                await _context.SaveChangesAsync();
                return Ok(ApiResponse<object>.CreateSuccess(null, "Quiz deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.CreateFailure($"Error deleting quiz: {ex.Message}"));
            }
        }
    }
}
