using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.API.Data;
using System.Security.Claims;
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

        // -----------------------------
        // GET: api/quizzes
        // -----------------------------
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Quiz>>>> GetAll()
        {
            try
            {
                var quizzes = await _context.Quizzes
                                            .Include(q => q.QuizQuestions)
                                                .ThenInclude(qq => qq.Question)
                                            .Include(q => q.Assignments)
                                                .ThenInclude(a => a.User)
                                            .ToListAsync();
                return Ok(ApiResponse<List<Quiz>>.CreateSuccess(quizzes, "Quizzes retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<Quiz>>.CreateFailure($"Error retrieving quizzes: {ex.Message}"));
            }
        }

        // -----------------------------
        // GET: api/quizzes/{id}
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Quiz>>> GetById(int id)
        {
            try
            {
                var quiz = await _context.Quizzes
                                         .Include(q => q.QuizQuestions)
                                             .ThenInclude(qq => qq.Question)
                                         .Include(q => q.Assignments)
                                             .ThenInclude(a => a.User)
                                         .FirstOrDefaultAsync(q => q.Id == id);

                if (quiz == null)
                    return NotFound(ApiResponse<Quiz>.CreateFailure("Quiz not found."));

                return Ok(ApiResponse<Quiz>.CreateSuccess(quiz, "Quiz retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Quiz>.CreateFailure($"Error retrieving quiz: {ex.Message}"));
            }
        }

        // -----------------------------
        // POST: api/quizzes
        // -----------------------------
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Quiz>>> Create([FromBody] Quiz quiz, [FromQuery] List<int>? questionIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(quiz.Title))
                    return BadRequest(ApiResponse<Quiz>.CreateFailure("Quiz title is required."));

                // Validate time range
                if (quiz.StartTime >= quiz.EndTime)
                    return BadRequest(ApiResponse<Quiz>.CreateFailure("End time must be after start time."));

                // Add quiz
                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();

                // Assign questions
                if (questionIds != null && questionIds.Any())
                {
                    foreach (var qId in questionIds)
                    {
                        var question = await _context.Questions.FindAsync(qId);
                        if (question != null)
                        {
                            _context.QuizQuestions.Add(new QuizQuestion
                            {
                                QuizId = quiz.Id,
                                QuestionId = qId
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Reload the quiz with related data
                var createdQuiz = await _context.Quizzes
                                                .Include(q => q.QuizQuestions)
                                                    .ThenInclude(qq => qq.Question)
                                                .FirstOrDefaultAsync(q => q.Id == quiz.Id);

                return CreatedAtAction(nameof(GetById), new { id = quiz.Id },
                    ApiResponse<Quiz>.CreateSuccess(createdQuiz, "Quiz created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Quiz>.CreateFailure($"Error creating quiz: {ex.Message}"));
            }
        }

        // -----------------------------
        // PUT: api/quizzes/{id}
        // -----------------------------
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] Quiz updatedQuiz)
        {
            try
            {
                var quiz = await _context.Quizzes.FindAsync(id);
                if (quiz == null)
                    return NotFound(ApiResponse<object>.CreateFailure("Quiz not found."));

                if (string.IsNullOrWhiteSpace(updatedQuiz.Title))
                    return BadRequest(ApiResponse<object>.CreateFailure("Quiz title is required."));

                // Validate time range
                if (updatedQuiz.StartTime >= updatedQuiz.EndTime)
                    return BadRequest(ApiResponse<object>.CreateFailure("End time must be after start time."));

                quiz.Title = updatedQuiz.Title;
                quiz.StartTime = updatedQuiz.StartTime;
                quiz.EndTime = updatedQuiz.EndTime;

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.CreateSuccess(null, "Quiz updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.CreateFailure($"Error updating quiz: {ex.Message}"));
            }
        }

        // -----------------------------
        // DELETE: api/quizzes/{id}
        // -----------------------------
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

        // -----------------------------
        // Assign quiz to user
        // -----------------------------
        [HttpPost("{quizId}/assign/{userId}")]
        public async Task<ActionResult<ApiResponse<QuizAssignment>>> AssignQuiz(int quizId, int userId)
        {
            try
            {
                var quiz = await _context.Quizzes.FindAsync(quizId);
                var user = await _context.Users.FindAsync(userId);

                if (quiz == null || user == null)
                    return BadRequest(ApiResponse<QuizAssignment>.CreateFailure("Invalid quiz or user ID."));

                if (await _context.QuizAssignments.AnyAsync(a => a.QuizId == quizId && a.UserId == userId))
                    return BadRequest(ApiResponse<QuizAssignment>.CreateFailure("Quiz already assigned to this user."));

                var assignment = new QuizAssignment
                {
                    QuizId = quizId,
                    UserId = userId,
                    Completed = false
                };

                _context.QuizAssignments.Add(assignment);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<QuizAssignment>.CreateSuccess(assignment, "Quiz assigned successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QuizAssignment>.CreateFailure($"Error assigning quiz: {ex.Message}"));
            }
        }

        // -----------------------------
        // Remove quiz assignment from user
        // -----------------------------
        [HttpDelete("{quizId}/assign/{userId}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveAssignment(int quizId, int userId)
        {
            try
            {
                var assignment = await _context.QuizAssignments
                                               .FirstOrDefaultAsync(a => a.QuizId == quizId && a.UserId == userId);

                if (assignment == null)
                    return NotFound(ApiResponse<object>.CreateFailure("Assignment not found."));

                _context.QuizAssignments.Remove(assignment);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.CreateSuccess(null, "Assignment removed successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.CreateFailure($"Error removing assignment: {ex.Message}"));
            }
        }

        // GET: api/user/quizzes/all-assigned
        [HttpGet("all-assigned")]
        public async Task<ActionResult<ApiResponse<List<AssignedQuizDetailDto>>>> GetAllAssignedQuizzes()
        {
            try
            {
                var now = DateTime.UtcNow;

                var assignments = await _context.QuizAssignments
                    .Include(a => a.User)
                    .Include(a => a.Quiz)
                        .ThenInclude(q => q.Category)
                    .Select(a => new
                    {
                        a.UserId,
                        Username = a.User.Username,
                        a.QuizId,
                        a.Quiz.Title,
                        CategoryName = a.Quiz.Category.Name,
                        a.Quiz.StartTime,
                        a.Quiz.EndTime,
                        a.Completed,
                        Result = _context.QuizResults
                            .Where(r => r.UserId == a.UserId && r.QuizId == a.QuizId)
                            .Select(r => new { r.Score, r.TotalQuestions, r.TakenAt })
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                var result = assignments.Select(a =>
                {
                    string status;
                    if (a.Completed && a.Result != null)
                        status = "Completed";
                    else if (a.EndTime < now)
                        status = "Missed";
                    else if (a.StartTime > now)
                        status = "Upcoming";
                    else
                        status = "Available";

                    return new AssignedQuizDetailDto
                    {
                        QuizId = a.QuizId,
                        Title = a.Title,
                        CategoryName = a.CategoryName,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        IsCompleted = a.Completed,
                        Score = a.Result?.Score,
                        TotalQuestions = a.Result?.TotalQuestions,
                        CompletedAt = a.Result?.TakenAt,
                        Status = status,
                        AssignedToUserId = a.UserId,
                        AssignedToUsername = a.Username
                    };
                }).ToList();

                return Ok(ApiResponse<List<AssignedQuizDetailDto>>.CreateSuccess(result, "All assigned quizzes retrieved."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<AssignedQuizDetailDto>>.CreateFailure($"Error: {ex.Message}"));
            }
        }

    }
}