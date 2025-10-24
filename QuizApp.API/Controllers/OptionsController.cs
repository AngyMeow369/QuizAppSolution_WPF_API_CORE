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
        public async Task<IActionResult> GetAll()
        {
            var options = await _context.Options
                                        .Include(o => o.Question)
                                        .ToListAsync();
            return Ok(options);
        }

        // -----------------------------
        // GET: api/options/{id}
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var option = await _context.Options
                                       .Include(o => o.Question)
                                       .FirstOrDefaultAsync(o => o.Id == id);
            if (option == null) return NotFound();
            return Ok(option);
        }

        // -----------------------------
        // POST: api/options
        // -----------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Option option)
        {
            if (string.IsNullOrWhiteSpace(option.Text))
                return BadRequest("Option text is required.");

            var question = await _context.Questions.FindAsync(option.QuestionId);
            if (question == null)
                return BadRequest("Invalid QuestionId.");

            _context.Options.Add(option);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = option.Id }, option);
        }

        // -----------------------------
        // PUT: api/options/{id}
        // -----------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Option updatedOption)
        {
            var option = await _context.Options.FindAsync(id);
            if (option == null) return NotFound();

            if (string.IsNullOrWhiteSpace(updatedOption.Text))
                return BadRequest("Option text is required.");

            option.Text = updatedOption.Text;
            option.IsCorrect = updatedOption.IsCorrect;
            option.QuestionId = updatedOption.QuestionId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // -----------------------------
        // DELETE: api/options/{id}
        // -----------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var option = await _context.Options.FindAsync(id);
            if (option == null) return NotFound();

            _context.Options.Remove(option);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
