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
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<User>>>> GetAll()
        {
            try
            {
                var users = await _context.Users
                                          .Include(u => u.QuizAssignments)
                                          .Include(u => u.QuizResults)
                                          .ToListAsync();
                return Ok(ApiResponse<List<User>>.CreateSuccess(users, "Users retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<User>>.CreateFailure($"Error retrieving users: {ex.Message}"));
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<User>>> GetById(int id)
        {
            try
            {
                var user = await _context.Users
                                         .Include(u => u.QuizAssignments)
                                         .Include(u => u.QuizResults)
                                         .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    return NotFound(ApiResponse<User>.CreateFailure("User not found."));

                return Ok(ApiResponse<User>.CreateSuccess(user, "User retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<User>.CreateFailure($"Error retrieving user: {ex.Message}"));
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<ApiResponse<User>>> Create([FromBody] User user)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Role))
                    return BadRequest(ApiResponse<User>.CreateFailure("Username and Role are required."));

                if (string.IsNullOrWhiteSpace(user.PasswordHash))
                    return BadRequest(ApiResponse<User>.CreateFailure("Password is required."));

                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                    return BadRequest(ApiResponse<User>.CreateFailure("Username already exists."));

                // Hash password using BCrypt (consistent with AuthController)
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                user.CreatedAt = DateTime.UtcNow;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Clear password hash for security before returning
                user.PasswordHash = string.Empty;

                return CreatedAtAction(nameof(GetById), new { id = user.Id },
                    ApiResponse<User>.CreateSuccess(user, "User created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<User>.CreateFailure($"Error creating user: {ex.Message}"));
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] User updatedUser)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(ApiResponse<object>.CreateFailure("User not found."));

                // Update username if provided
                if (!string.IsNullOrWhiteSpace(updatedUser.Username))
                {
                    // Check if new username already exists (excluding current user)
                    if (await _context.Users.AnyAsync(u => u.Username == updatedUser.Username && u.Id != id))
                        return BadRequest(ApiResponse<object>.CreateFailure("Username already exists."));

                    user.Username = updatedUser.Username;
                }

                // Update role if provided
                if (!string.IsNullOrWhiteSpace(updatedUser.Role))
                    user.Role = updatedUser.Role;

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(updatedUser.PasswordHash))
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedUser.PasswordHash);

                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.CreateSuccess(null, "User updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.CreateFailure($"Error updating user: {ex.Message}"));
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var user = await _context.Users
                                         .Include(u => u.QuizAssignments)
                                         .Include(u => u.QuizResults)
                                         .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    return NotFound(ApiResponse<object>.CreateFailure("User not found."));

                // Remove related assignments and results
                _context.QuizAssignments.RemoveRange(user.QuizAssignments);
                _context.QuizResults.RemoveRange(user.QuizResults);

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.CreateSuccess(null, "User deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.CreateFailure($"Error deleting user: {ex.Message}"));
            }
        }
    }
}