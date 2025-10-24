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
        public async Task<IActionResult> GetAll()
        {
            var questions = await _context.Questions
                                          .Include(q => q.Category)
                                          .Include(q => q.Options)
                                          .ToListAsync();
            return Ok(questions);
        }

        // -----------------------------
        // GET: api/questions/{id}
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _context.Questions
                                         .Include(q => q.Category)
                                         .Include(q => q.Options)
                                         .FirstOrDefaultAsync(q => q.Id == id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        // -----------------------------
        // POST: api/questions
        // -----------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Question question)
        {
            if (string.IsNullOrWhiteSpace(question.Text))
                return BadRequest("Question text is required.");

            var category = await _context.Categories.FindAsync(question.CategoryId);
            if (category == null)
                return BadRequest("Invalid category ID.");

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = question.Id }, question);
        }

        // -----------------------------
        // PUT: api/questions/{id}
        // -----------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Question updatedQuestion)
        {
            var question = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id);
            if (question == null) return NotFound();

            if (string.IsNullOrWhiteSpace(updatedQuestion.Text))
                return BadRequest("Question text is required.");

            var category = await _context.Categories.FindAsync(updatedQuestion.CategoryId);
            if (category == null)
                return BadRequest("Invalid category ID.");

            question.Text = updatedQuestion.Text;
            question.CategoryId = updatedQuestion.CategoryId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // -----------------------------
        // DELETE: api/questions/{id}
        // -----------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id);
            if (question == null) return NotFound();

            // Remove related options first
            _context.Options.RemoveRange(question.Options);
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
