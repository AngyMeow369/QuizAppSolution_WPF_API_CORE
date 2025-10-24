using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QuizApp.API.Data;
using QuizApp.API.Models;

namespace QuizApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                                      .Include(u => u.QuizAssignments)
                                      .Include(u => u.QuizResults)
                                      .ToListAsync();
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users
                                     .Include(u => u.QuizAssignments)
                                     .Include(u => u.QuizResults)
                                     .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Role))
                return BadRequest("Username and Role are required.");

            // Hash password
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
                return BadRequest("Password is required.");

            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(updatedUser.Username))
                user.Username = updatedUser.Username;

            if (!string.IsNullOrWhiteSpace(updatedUser.Role))
                user.Role = updatedUser.Role;

            if (!string.IsNullOrWhiteSpace(updatedUser.PasswordHash))
                user.PasswordHash = _passwordHasher.HashPassword(user, updatedUser.PasswordHash);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users
                                     .Include(u => u.QuizAssignments)
                                     .Include(u => u.QuizResults)
                                     .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            // Remove related assignments and results
            _context.QuizAssignments.RemoveRange(user.QuizAssignments);
            _context.QuizResults.RemoveRange(user.QuizResults);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
