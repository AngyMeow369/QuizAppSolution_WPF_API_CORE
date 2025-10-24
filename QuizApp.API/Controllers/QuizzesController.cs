using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.API.Data;
using QuizApp.API.Models;

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
        public async Task<IActionResult> GetAll()
        {
            var quizzes = await _context.Quizzes
                                        .Include(q => q.QuizQuestions)
                                            .ThenInclude(qq => qq.Question)
                                        .Include(q => q.Assignments)
                                            .ThenInclude(a => a.User)
                                        .ToListAsync();
            return Ok(quizzes);
        }

        // -----------------------------
        // GET: api/quizzes/{id}
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var quiz = await _context.Quizzes
                                     .Include(q => q.QuizQuestions)
                                         .ThenInclude(qq => qq.Question)
                                     .Include(q => q.Assignments)
                                         .ThenInclude(a => a.User)
                                     .FirstOrDefaultAsync(q => q.Id == id);
            if (quiz == null) return NotFound();
            return Ok(quiz);
        }

        // -----------------------------
        // POST: api/quizzes
        // -----------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Quiz quiz, [FromQuery] List<int>? questionIds)
        {
            if (string.IsNullOrWhiteSpace(quiz.Title))
                return BadRequest("Quiz title is required.");

            // Add quiz
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            // Assign questions
            if (questionIds != null)
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

            return CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz);
        }

        // -----------------------------
        // PUT: api/quizzes/{id}
        // -----------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Quiz updatedQuiz)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            quiz.Title = updatedQuiz.Title;
            quiz.StartTime = updatedQuiz.StartTime;
            quiz.EndTime = updatedQuiz.EndTime;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // -----------------------------
        // DELETE: api/quizzes/{id}
        // -----------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var quiz = await _context.Quizzes
                                     .Include(q => q.QuizQuestions)
                                     .Include(q => q.Assignments)
                                     .FirstOrDefaultAsync(q => q.Id == id);
            if (quiz == null) return NotFound();

            _context.QuizQuestions.RemoveRange(quiz.QuizQuestions);
            _context.QuizAssignments.RemoveRange(quiz.Assignments);
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // -----------------------------
        // Assign quiz to user
        // -----------------------------
        [HttpPost("{quizId}/assign/{userId}")]
        public async Task<IActionResult> AssignQuiz(int quizId, int userId)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            var user = await _context.Users.FindAsync(userId);

            if (quiz == null || user == null)
                return BadRequest("Invalid quiz or user ID.");

            if (await _context.QuizAssignments.AnyAsync(a => a.QuizId == quizId && a.UserId == userId))
                return BadRequest("Quiz already assigned to this user.");

            var assignment = new QuizAssignment
            {
                QuizId = quizId,
                UserId = userId,
                Completed = false
            };

            _context.QuizAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return Ok(assignment);
        }

        // -----------------------------
        // Remove quiz assignment from user
        // -----------------------------
        [HttpDelete("{quizId}/assign/{userId}")]
        public async Task<IActionResult> RemoveAssignment(int quizId, int userId)
        {
            var assignment = await _context.QuizAssignments
                                           .FirstOrDefaultAsync(a => a.QuizId == quizId && a.UserId == userId);
            if (assignment == null) return NotFound();

            _context.QuizAssignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
