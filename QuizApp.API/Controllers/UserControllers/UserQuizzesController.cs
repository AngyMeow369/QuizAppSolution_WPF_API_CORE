using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.API.Data;
using QuizApp.API.Models;
using QuizApp.Shared.DTOs;
using System.Security.Claims;

namespace QuizApp.API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/user/[controller]")]
    [Authorize(Roles = "User")]
    public class QuizzesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuizzesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/user/quizzes/my-assigned
        [HttpGet("my-assigned")]
        public async Task<ActionResult<ApiResponse<List<QuizDto>>>> GetMyAssignedQuizzes()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var quizzes = await _context.QuizAssignments
                    .Where(a => a.UserId == userId && !a.Completed)
                    .Include(a => a.Quiz)
                    .Select(a => new QuizDto
                    {
                        Id = a.Quiz.Id,
                        Title = a.Quiz.Title,
                        StartTime = a.Quiz.StartTime,
                        EndTime = a.Quiz.EndTime
                    })
                    .Where(q => q.StartTime <= DateTime.UtcNow && q.EndTime >= DateTime.UtcNow)
                    .ToListAsync();

                return Ok(ApiResponse<List<QuizDto>>.CreateSuccess(quizzes, "Assigned quizzes retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<QuizDto>>.CreateFailure($"Error retrieving assigned quizzes: {ex.Message}"));
            }
        }

        // GET: api/user/quizzes/{id}/take
        [HttpGet("{id}/take")]
        public async Task<ActionResult<ApiResponse<QuizTakeDto>>> GetQuizForTaking(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                // Verify user is assigned this quiz and hasn't completed it
                var assignment = await _context.QuizAssignments
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.QuizId == id && !a.Completed);

                if (assignment == null)
                    return NotFound(ApiResponse<QuizTakeDto>.CreateFailure("Quiz not found or already completed."));

                var quiz = await _context.Quizzes
                    .Include(q => q.QuizQuestions)
                        .ThenInclude(qq => qq.Question)
                            .ThenInclude(q => q.Options)
                    .Include(q => q.QuizQuestions)
                        .ThenInclude(qq => qq.Question)
                            .ThenInclude(q => q.Category)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quiz == null)
                    return NotFound(ApiResponse<QuizTakeDto>.CreateFailure("Quiz not found."));

                // Return quiz without correct answers
                var quizTakeDto = new QuizTakeDto
                {
                    Id = quiz.Id,
                    Title = quiz.Title,
                    StartTime = quiz.StartTime,
                    EndTime = quiz.EndTime,
                    Questions = quiz.QuizQuestions.Select(qq => new QuestionTakeDto
                    {
                        Id = qq.Question.Id,
                        Text = qq.Question.Text,
                        CategoryName = qq.Question.Category.Name,
                        Options = qq.Question.Options.Select(o => new OptionTakeDto
                        {
                            Id = o.Id,
                            Text = o.Text
                            // Don't include IsCorrect for security
                        }).ToList()
                    }).ToList()
                };

                return Ok(ApiResponse<QuizTakeDto>.CreateSuccess(quizTakeDto, "Quiz retrieved for taking."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QuizTakeDto>.CreateFailure($"Error retrieving quiz: {ex.Message}"));
            }
        }

        // POST: api/user/quizzes/{id}/submit
        [HttpPost("{id}/submit")]
        public async Task<ActionResult<ApiResponse<QuizResultDto>>> SubmitQuiz(int id, [FromBody] QuizSubmissionDto submission)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                // Verify user is assigned this quiz
                var assignment = await _context.QuizAssignments
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.QuizId == id && !a.Completed);

                if (assignment == null)
                    return BadRequest(ApiResponse<QuizResultDto>.CreateFailure("Quiz not found or already completed."));

                // Calculate score
                var quiz = await _context.Quizzes
                    .Include(q => q.QuizQuestions)
                        .ThenInclude(qq => qq.Question)
                            .ThenInclude(q => q.Options)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quiz == null)
                    return NotFound(ApiResponse<QuizResultDto>.CreateFailure("Quiz not found."));

                int score = 0;
                int totalQuestions = quiz.QuizQuestions.Count;

                foreach (var questionSubmission in submission.Answers)
                {
                    var question = quiz.QuizQuestions
                        .FirstOrDefault(qq => qq.QuestionId == questionSubmission.QuestionId)?.Question;

                    if (question != null)
                    {
                        var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                        if (correctOption != null && correctOption.Id == questionSubmission.SelectedOptionId)
                        {
                            score++;
                        }
                    }
                }

                // Create quiz result
                var quizResult = new QuizResult
                {
                    QuizId = id,
                    UserId = userId,
                    Score = score,
                    TotalQuestions = totalQuestions,
                    TakenAt = DateTime.UtcNow
                };

                // Mark assignment as completed
                assignment.Completed = true;

                _context.QuizResults.Add(quizResult);
                await _context.SaveChangesAsync();

                var resultDto = new QuizResultDto
                {
                    Id = quizResult.Id,
                    QuizId = quizResult.QuizId,
                    QuizTitle = quiz.Title,
                    Score = quizResult.Score,
                    TotalQuestions = quizResult.TotalQuestions,
                    TakenAt = quizResult.TakenAt
                };

                return Ok(ApiResponse<QuizResultDto>.CreateSuccess(resultDto, "Quiz submitted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QuizResultDto>.CreateFailure($"Error submitting quiz: {ex.Message}"));
            }
        }
    }
}