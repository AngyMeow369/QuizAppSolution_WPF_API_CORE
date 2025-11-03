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
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/user/results/my-results
        [HttpGet("my-results")]
        public async Task<ActionResult<ApiResponse<List<QuizResultDto>>>> GetMyResults()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var results = await _context.QuizResults
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Quiz)
                    .OrderByDescending(r => r.TakenAt)
                    .Select(r => new QuizResultDto
                    {
                        Id = r.Id,
                        QuizId = r.QuizId,
                        QuizTitle = r.Quiz.Title,
                        Score = r.Score,
                        TotalQuestions = r.TotalQuestions,
                        TakenAt = r.TakenAt
                    })
                    .ToListAsync();

                return Ok(ApiResponse<List<QuizResultDto>>.CreateSuccess(results, "Quiz results retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<QuizResultDto>>.CreateFailure($"Error retrieving results: {ex.Message}"));
            }
        }

        // GET: api/user/results/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QuizResultDto>>> GetResultById(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var result = await _context.QuizResults
                    .Where(r => r.Id == id && r.UserId == userId)
                    .Include(r => r.Quiz)
                    .Select(r => new QuizResultDto
                    {
                        Id = r.Id,
                        QuizId = r.QuizId,
                        QuizTitle = r.Quiz.Title,
                        Score = r.Score,
                        TotalQuestions = r.TotalQuestions,
                        TakenAt = r.TakenAt
                    })
                    .FirstOrDefaultAsync();

                if (result == null)
                    return NotFound(ApiResponse<QuizResultDto>.CreateFailure("Result not found."));

                return Ok(ApiResponse<QuizResultDto>.CreateSuccess(result, "Result retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QuizResultDto>.CreateFailure($"Error retrieving result: {ex.Message}"));
            }
        }
    }
}