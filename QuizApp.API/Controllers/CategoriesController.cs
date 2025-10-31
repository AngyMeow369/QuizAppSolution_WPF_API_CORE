using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.API.Data;
using QuizApp.API.DTOs;
using QuizApp.API.Models;
using QuizApp.Shared.DTOs;

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
        public async Task<ActionResult<ApiResponse<List<Category>>>> GetAll()
        {
            try
            {
                var categories = await _context.Categories
                                               .Include(c => c.Questions)
                                               .ToListAsync();
                return Ok(ApiResponse<List<Category>>.CreateSuccess(categories, "Categories retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<Category>>.CreateFailure($"Error retrieving categories: {ex.Message}"));
            }
        }

        // -----------------------------
        // GET: api/categories/{id}
        // -----------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Category>>> GetById(int id)
        {
            try
            {
                var category = await _context.Categories
                                             .Include(c => c.Questions)
                                             .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return NotFound(ApiResponse<Category>.CreateFailure("Category not found."));

                return Ok(ApiResponse<Category>.CreateSuccess(category, "Category retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Category>.CreateFailure($"Error retrieving category: {ex.Message}"));
            }
        }

        // -----------------------------
        // POST: api/categories
        // -----------------------------
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Category>>> Create([FromBody] CreateCategoryDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest(ApiResponse<Category>.CreateFailure("Category name is required."));

                // Check for duplicate category name
                if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower()))
                    return BadRequest(ApiResponse<Category>.CreateFailure("Category name already exists."));

                var category = new Category { Name = dto.Name };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                // Reload with related data
                var createdCategory = await _context.Categories
                                                    .Include(c => c.Questions)
                                                    .FirstOrDefaultAsync(c => c.Id == category.Id);

                return CreatedAtAction(nameof(GetById), new { id = category.Id },
                    ApiResponse<Category>.CreateSuccess(createdCategory, "Category created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Category>.CreateFailure($"Error creating category: {ex.Message}"));
            }
        }


        // -----------------------------
        // PUT: api/categories/{id}
        // -----------------------------
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                    return NotFound(ApiResponse<object>.CreateFailure("Category not found."));

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest(ApiResponse<object>.CreateFailure("Category name is required."));

                // Check for duplicate name
                if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && c.Id != id))
                    return BadRequest(ApiResponse<object>.CreateFailure("Category name already exists."));

                category.Name = dto.Name;
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.CreateSuccess(null, "Category updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.CreateFailure($"Error updating category: {ex.Message}"));
            }
        }


        // -----------------------------
        // DELETE: api/categories/{id}
        // -----------------------------
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var category = await _context.Categories
                                             .Include(c => c.Questions)
                                             .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                    return NotFound(ApiResponse<object>.CreateFailure("Category not found."));

                // Check if category has questions
                if (category.Questions.Any())
                    return BadRequest(ApiResponse<object>.CreateFailure("Cannot delete category that has questions. Remove or reassign questions first."));

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<object>.CreateSuccess(null, "Category deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.CreateFailure($"Error deleting category: {ex.Message}"));
            }
        }
    }
}