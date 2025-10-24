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
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -----------------------------
        // GET: api/categories
        // -----------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                                           .Include(c => c.Questions)
                                           .ToListAsync();
            return Ok(categories);
        }

        // -----------------------------
        // GET: api/categories/{id}
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories
                                         .Include(c => c.Questions)
                                         .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        // -----------------------------
        // POST: api/categories
        // -----------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Category name is required.");

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        // -----------------------------
        // PUT: api/categories/{id}
        // -----------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category updatedCategory)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            if (string.IsNullOrWhiteSpace(updatedCategory.Name))
                return BadRequest("Category name is required.");

            category.Name = updatedCategory.Name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // -----------------------------
        // DELETE: api/categories/{id}
        // -----------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
